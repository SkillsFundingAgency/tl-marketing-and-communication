using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using sfa.Tl.Marketing.Communication.Application.Caching;
using sfa.Tl.Marketing.Communication.Application.ClassMaps;
using sfa.Tl.Marketing.Communication.Application.Extensions;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models.Configuration;
using sfa.Tl.Marketing.Communication.Models.Dto;
using sfa.Tl.Marketing.Communication.Models.Enums;
using sfa.Tl.Marketing.Communication.Models.Extensions;

namespace sfa.Tl.Marketing.Communication.Application.Services;

public class TownDataService : ITownDataService
{
    private readonly int _cacheExpiryInSeconds;

    private readonly IMemoryCache _cache;
    private readonly HttpClient _httpClient;
    private readonly ITableStorageService _tableStorageService;
    private readonly ILogger<TownDataService> _logger;

    //See user guide for details on the fields in this data
    // 2016 - https://geoportal.statistics.gov.uk/datasets/index-of-place-names-in-great-britain-november-2021-user-guide/about
    // 2021 - https://geoportal.statistics.gov.uk/datasets/index-of-place-names-in-great-britain-november-2021-user-guide/about
    //        https://geoportal.statistics.gov.uk/datasets/ons::index-of-place-names-in-great-britain-july-2016-table/about
    public const string OfficeForNationalStatisticsLocationUrl = "https://services1.arcgis.com/ESMARspQHYMw9BZ9/arcgis/rest/services/IPN_GB_2016/FeatureServer/0/query?where=ctry15nm%20%3D%20'ENGLAND'%20AND%20popcnt%20%3E%3D%20500%20AND%20popcnt%20%3C%3D%2010000000&outFields=placeid,place15nm,ctry15nm,cty15nm,ctyltnm,lad15nm,laddescnm,pcon15nm,lat,long,popcnt,descnm&returnDistinctValues=true&outSR=4326&f=json";
    //public const string OfficeForNationalStatisticsLocationUrl = "https://services1.arcgis.com/ESMARspQHYMw9BZ9/arcgis/rest/services/Index_of_Place_Names_in_Great_Britain_July_2016_2022/FeatureServer/0/query?where=ctry15nm%20%3D%20'ENGLAND'%20AND%20popcnt%20%3E%3D%20500%20AND%20popcnt%20%3C%3D%2010000000&outFields=placeid,place15nm,ctry15nm,cty15nm,ctyltnm,lad15nm,laddescnm,pcon15nm,lat,long,popcnt,descnm&returnDistinctValues=true&outSR=4326&f=geojson";

    public const int CountyMaxLength = 50;
    public const int LocalAuthorityMaxLength = 50;
    public const int LocationNameMaxLength = 400;

    public static Uri GetUri(int offset, int recordSize) =>
        new($"{OfficeForNationalStatisticsLocationUrl}&resultRecordCount={recordSize}&resultOffSet={offset}");

    public TownDataService(
        HttpClient httpClient,
        ITableStorageService tableStorageService,
        IMemoryCache cache,
        ConfigurationOptions configuration,
        ILogger<TownDataService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _tableStorageService = tableStorageService ?? throw new ArgumentNullException(nameof(tableStorageService));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        if (configuration == null) throw new ArgumentNullException(nameof(configuration));
        _cacheExpiryInSeconds = configuration.CacheExpiryInSeconds;
    }

    public async Task<int> ImportTowns()
    {
        var items = await ReadOnsLocationData();

        var towns = ConvertToTowns(items);

        return await SaveToTableStorage(towns);
    }

    public async Task<int> ImportTownsFromCsvStream(Stream stream)
    {
        using var reader = new StreamReader(stream);
        using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
        csvReader.Context.RegisterClassMap<OnsLocationApiItemClassMap>();

        var items = csvReader
            .GetRecords<OnsLocationApiItem>()
            .Where(item => string.Compare(item.Country, "England", StringComparison.InvariantCultureIgnoreCase) == 0
                           && item.PopulationCount is >= 500 and <= 10000000)
            .ToList();

        var towns = ConvertToTowns(items);

        return await SaveToTableStorage(towns);
    }

    public async Task<(bool IsValid, Town Town)> IsSearchTermValid(string searchTerm)
    {
        var searchString = searchTerm.ToSearchableString();
        var partitionKey = searchString[..Math.Min(3, searchString.Length)].ToUpper();
        var cacheKey = CacheKeys.TownPartitionKey(partitionKey);

        if (!_cache.TryGetValue(cacheKey, out IList<Town> towns))
        {
            towns = await _tableStorageService.GetTownsByPartitionKey(partitionKey);
            if (_cacheExpiryInSeconds > 0 && towns.Any())
            {
                _cache.Set(cacheKey, towns, CacheUtilities.DefaultMemoryCacheEntryOptions(_cacheExpiryInSeconds));
            }
        }

        var town = towns.Where(t =>
            t.SearchString != null && t.SearchString.StartsWith(searchString))
            .MinBy(t => t.Name);

        return (town != null, town);
    }

    public async Task<IEnumerable<Town>> Search(string searchTerm, int maxResults)
    {
        if (searchTerm.IsFullOrPartialPostcode())
        {
            return new List<Town>();
        }

        var searchString = searchTerm.ToSearchableString();
        var partitionKey = searchString[..Math.Min(3, searchString.Length)].ToUpper();
        var cacheKey = CacheKeys.TownPartitionKey(partitionKey);

        if (!_cache.TryGetValue(cacheKey, out IList<Town> towns))
        {
            towns = await _tableStorageService.GetTownsByPartitionKey(partitionKey);
            if (_cacheExpiryInSeconds > 0 && towns.Any())
            {
                _cache.Set(cacheKey, towns, CacheUtilities.DefaultMemoryCacheEntryOptions(_cacheExpiryInSeconds));
            }
        }

        return towns.Where(t =>
                t.SearchString != null && t.SearchString.StartsWith(searchString))
            .OrderBy(t => t.Name)
            .Take(maxResults);
    }

    private async Task<IEnumerable<OnsLocationApiItem>> ReadOnsLocationData()
    {
        var offSet = 0;
        const int recordSize = 2000;
        var moreData = true;

        var items = new List<OnsLocationApiItem>();

        while (moreData)
        {
            var uri = GetUri(offSet, recordSize);
            var responseMessage = await _httpClient.GetAsync(uri);

            if (responseMessage.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError("National Statistics API call to '{uri}' failed with " +
                                 "{StatusCode} - {ReasonPhrase}",
                    uri, responseMessage.StatusCode, responseMessage.ReasonPhrase);
            }

            responseMessage.EnsureSuccessStatusCode();

            (var responseItems, moreData) =
                await ReadOnsLocationApiDataResponse(
                    responseMessage);

            offSet += recordSize;

            items.AddRange(responseItems);
        }

        return items;
    }

    private static List<Town> ConvertToTowns(IEnumerable<OnsLocationApiItem> items)
    {
        var towns = items
            .Where(item => !string.IsNullOrEmpty(item.LocationName) &&
                           !string.IsNullOrEmpty(item.LocalAuthorityName) &&
                           !string.IsNullOrEmpty(item.LocationAuthorityDistrict) &&
                           item.PlaceName != PlaceNameDescription.None)
            .GroupBy(c => new
            {
                c.LocalAuthorityName,
                Name = c.LocationName,
                c.LocationAuthorityDistrict
            })
            .Select(item => item.First())
            .GroupBy(c => new
            {
                c.Id
            })
            .Select(SelectDuplicateByLocalAuthorityDistrictDescription)
            .Select(item => new Town
            {
                Id = item.Id,
                Name = item.LocationName,
                County = item.CountyName,
                LocalAuthority = item.LocalAuthorityName,
                Latitude = item.Latitude,
                Longitude = item.Longitude,
                SearchString =
                    $"{item.LocationName}{item.CountyName ?? item.LocalAuthorityName}"
                        .ToSearchableString()
            })
            .ToList();
        return towns;
    }

    private static async Task<(IEnumerable<OnsLocationApiItem>, bool)> ReadOnsLocationApiDataResponse(
        HttpResponseMessage responseMessage)
    {
        var jsonDocument = await JsonDocument.ParseAsync(await responseMessage.Content.ReadAsStreamAsync());

        var root = jsonDocument.RootElement;

        var exceededTransferLimit = root
                                        .TryGetProperty("exceededTransferLimit", out var property)
                                    && property.GetBoolean();

        var towns =
            root
            .GetProperty("features")
            .EnumerateArray()
            .Select(attr => new { attributeElement = attr.GetProperty("attributes") })
            .Select(x => new OnsLocationApiItem
            {
                // ReSharper disable StringLiteralTypo
                Id = x.attributeElement.SafeGetInt32("placeid"),
                LocationName = x.attributeElement.SafeGetString("place15nm", LocationNameMaxLength),
                CountyName = x.attributeElement.SafeGetString("cty15nm", CountyMaxLength),
                LocalAuthorityName = x.attributeElement.SafeGetString("ctyltnm", LocalAuthorityMaxLength),
                LocalAuthorityDistrictDescription = x.attributeElement.SafeGetString("laddescnm"),
                LocalAuthorityDistrict =
                    Enum.TryParse<LocalAuthorityDistrict>(x.attributeElement.SafeGetString("laddescnm"), out var localAuthorityDistrict)
                        ? localAuthorityDistrict : default,
                LocationAuthorityDistrict = x.attributeElement.SafeGetString("lad15nm"),
                PlaceNameDescription = x.attributeElement.SafeGetString("descnm"),
                PlaceName =
                    Enum.TryParse<PlaceNameDescription>(
                        x.attributeElement.SafeGetString("descnm"), out var placeName)
                    ? placeName : default,
                Latitude = x.attributeElement.SafeGetDouble("lat"),
                Longitude = x.attributeElement.SafeGetDouble("long")
                //ReSharper restore StringLiteralTypo
            });

        return (towns, exceededTransferLimit);
    }

    private static OnsLocationApiItem SelectDuplicateByLocalAuthorityDistrictDescription(IEnumerable<OnsLocationApiItem> items)
    {
        var values = items.ToList();

        if (values.Count > 1)
        {
            var orderByDescending = values.OrderByDescending(c => c.LocalAuthorityDistrict).ToList();
            var locationApiItem = orderByDescending.FirstOrDefault(c => !string.IsNullOrEmpty(c.LocalAuthorityDistrictDescription)
                                                                        && c.LocationName.Equals(c.LocationAuthorityDistrict, StringComparison.CurrentCultureIgnoreCase));
            if (locationApiItem != null)
            {
                return locationApiItem;
            }
            return orderByDescending
                .FirstOrDefault(c => !string.IsNullOrEmpty(c.LocalAuthorityDistrictDescription));
        }

        return values.FirstOrDefault();
    }

    private async Task<int> SaveToTableStorage(IList<Town> towns)
    {
        var removedTowns = await _tableStorageService.ClearTowns();
        _logger.LogInformation("Removed {removedTowns} towns from table storage", removedTowns);

        var savedTowns = await _tableStorageService.SaveTowns(towns);
        _logger.LogInformation("Saved {savedTowns} towns to table storage", savedTowns);

        return savedTowns;
    }
}
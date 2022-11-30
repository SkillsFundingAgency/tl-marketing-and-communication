using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using sfa.Tl.Marketing.Communication.Application.Extensions;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models.Dto;
using sfa.Tl.Marketing.Communication.Models.Enums;

namespace sfa.Tl.Marketing.Communication.Application.Services;

public class TownDataService : ITownDataService
{
    private readonly HttpClient _httpClient;
    //private readonly ITownRepository _townRepository;
    private readonly ILogger<TownDataService> _logger;

    //See user guide for details on the fields in this data
    // 2016 - https://geoportal.statistics.gov.uk/datasets/index-of-place-names-in-great-britain-november-2021-user-guide/about
    // 2021 - https://geoportal.statistics.gov.uk/datasets/index-of-place-names-in-great-britain-november-2021-user-guide/about
    public const string OfficeForNationalStatisticsLocationUrl = "https://services1.arcgis.com/ESMARspQHYMw9BZ9/arcgis/rest/services/IPN_GB_2016/FeatureServer/0/query?where=ctry15nm%20%3D%20'ENGLAND'%20AND%20popcnt%20%3E%3D%20500%20AND%20popcnt%20%3C%3D%2010000000&outFields=placeid,place15nm,ctry15nm,cty15nm,ctyltnm,lad15nm,laddescnm,pcon15nm,lat,long,popcnt,descnm&returnDistinctValues=true&outSR=4326&f=json";
    
    public const int CountyMaxLength = 50;
    public const int LocalAuthorityMaxLength = 50;
    public const int LocationNameMaxLength = 400;

    public static Uri GetUri(int offset, int recordSize) =>
        new($"{OfficeForNationalStatisticsLocationUrl}&resultRecordCount={recordSize}&resultOffSet={offset}");

    public TownDataService(
        HttpClient httpClient,
        //ITownRepository townRepository,
        ILogger<TownDataService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        //_townRepository = townRepository ?? throw new ArgumentNullException(nameof(townRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<int> ImportTowns()
    {
        var items = await ReadOnsLocationData();

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
                Longitude = item.Longitude
            })
            .ToList();

        //await _townRepository.Save(towns);

        return towns.Count;
    }

    public async Task<IEnumerable<Town>> Search(string searchTerm, int maxResults)
    {
        return new List<Town>
        {
            new() { Name = "Coventry" },
            new() { Name = "Oxford" }
        };
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

    private static async Task<(IEnumerable<OnsLocationApiItem>, bool)> ReadOnsLocationApiDataResponse(
        HttpResponseMessage responseMessage)
    {
        var jsonDocument = await JsonDocument.ParseAsync(await responseMessage.Content.ReadAsStreamAsync());

        var root = jsonDocument.RootElement;

        var exceededTransferLimit = root
                                        .TryGetProperty("exceededTransferLimit", out var property)
                                    && property.GetBoolean();

        var towns = //new List<LocationApiItem>();
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
                Latitude = x.attributeElement.SafeGetDecimal("lat"),
                Longitude = x.attributeElement.SafeGetDecimal("long")
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
}
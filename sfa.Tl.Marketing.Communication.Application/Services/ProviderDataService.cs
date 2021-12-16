using System;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using sfa.Tl.Marketing.Communication.Application.Extensions;
using sfa.Tl.Marketing.Communication.Models.Configuration;

namespace sfa.Tl.Marketing.Communication.Application.Services
{
    public class ProviderDataService : IProviderDataService
    {
        private const string ProviderTableDataCacheKey = "Provider_Table_Data";
        private const string QualificationTableDataCacheKey = "Qualification_Table_Data";
        private readonly int _cacheExpiryInSeconds;
        private readonly bool _mergeTempProviderData;

        private readonly IMemoryCache _cache;
        private readonly ITableStorageService _tableStorageService;

        private static readonly IDictionary<long, Provider> TempProviderData;

        static ProviderDataService()
        {
            TempProviderData = LoadTempProviderData();
        }

        public ProviderDataService(
            ITableStorageService tableStorageService,
            IMemoryCache cache,
            ConfigurationOptions configuration)
        {
            _tableStorageService = tableStorageService ?? throw new ArgumentNullException(nameof(tableStorageService));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _cacheExpiryInSeconds = configuration?.CacheExpiryInSeconds ?? 60;
            _mergeTempProviderData = configuration?.MergeTempProviderData ?? false;
        }

        public IQueryable<ProviderLocation> GetProviderLocations(int? qualificationId = null)
        {
            var providerLocations = new List<ProviderLocation>();

            var qualificationsDictionary = GetAllQualifications()
                .ToDictionary(q => q.Id);

            foreach (var provider in GetAllProviders())
            {
                var locations =
                    qualificationId > 0
                        ? provider.Locations
                            .Where(l => l.DeliveryYears.Any(d => d.Qualifications.Contains(qualificationId.Value)))
                            .ToList()
                        : provider.Locations;

                var currentProviderLocation =
                    locations.Select(l => new ProviderLocation
                    {
                        ProviderName = provider.Name,
                        Name = l.Name,
                        Latitude = l.Latitude,
                        Longitude = l.Longitude,
                        Postcode = l.Postcode,
                        Town = l.Town,
                        Website = l.Website,
                        DeliveryYears = l.DeliveryYears != null
                                ? l.DeliveryYears
                                    .Select(d => new DeliveryYear
                                    {
                                        Year = d.Year,
                                        Qualifications = GetQualificationsForDeliveryYear(d, qualificationsDictionary)
                                    })
                                    .OrderBy(d => d.Year)
                                    .ToList()
                                : new List<DeliveryYear>()
                    })
                        .Where(pl => pl.DeliveryYears.Any(y => y.Qualifications.Any()));

                providerLocations.AddRange(currentProviderLocation);
            }

            return providerLocations.AsQueryable();
        }

        public IEnumerable<Qualification> GetQualifications(int[] qualificationIds)
        {
            var qualifications = GetAllQualifications();
            return qualifications
                .Where(q => qualificationIds.Contains(q.Id))
                .OrderBy(q => q.Name);
        }

        public Qualification GetQualification(int qualificationId)
        {
            var qualifications = GetAllQualifications();
            return qualifications.SingleOrDefault(q => q.Id == qualificationId);
        }

        public IEnumerable<Qualification> GetQualifications()
        {
            var qualifications = GetAllQualifications();
            return qualifications
                .Append(new Qualification { Id = 0, Name = "All T Level courses" });
        }

        public IDictionary<string, string> GetWebsiteUrls()
        {
            var urlDictionary = new Dictionary<string, string>();

            foreach (var provider in GetAllProviders())
            {
                foreach (var location in
                    provider.Locations)
                {
                    if (!string.IsNullOrEmpty(location.Website))
                    {
                        //decode url for key because this will be compared to a decoded url later
                        var encodedUrl = WebUtility.UrlDecode(location.Website);
                        urlDictionary[encodedUrl!] = location.Website;
                    }
                }
            }

            return urlDictionary;
        }

        private IQueryable<Qualification> GetAllQualifications()
        {
            if (!_cache.TryGetValue(QualificationTableDataCacheKey,
                out IQueryable<Qualification> qualifications))
            {
                qualifications = _tableStorageService
                    .GetAllQualifications()
                    .GetAwaiter()
                    .GetResult()
                    .AsQueryable();
                _cache.Set(QualificationTableDataCacheKey, qualifications, GetCacheOptions());
            }

            return qualifications;
        }

        private static IList<Qualification> GetQualificationsForDeliveryYear(
            DeliveryYearDto deliveryYear,
            IDictionary<int, Qualification> qualificationsDictionary)
        {
            var list = new List<Qualification>();

            if (deliveryYear.Qualifications != null)
            {
                list.AddRange(
                    deliveryYear
                        .Qualifications
                        .Select(q => new Qualification
                        {
                            Id = q,
                            Name = qualificationsDictionary[q].Name,
                            Route = qualificationsDictionary[q].Route
                        }));
            }

            return list.OrderBy(q => q.Name).ToList();
        }

        private IQueryable<Provider> GetAllProviders()
        {
            if (!_cache.TryGetValue(ProviderTableDataCacheKey,
                out IQueryable<Provider> providers))
            {
                providers = _tableStorageService
                    .GetAllProviders()
                    .GetAwaiter()
                    .GetResult()
                    .AsQueryable();

                _cache.Set(ProviderTableDataCacheKey, providers, GetCacheOptions());
            }

            return providers;
        }

        private MemoryCacheEntryOptions GetCacheOptions()
        {
            var options = new MemoryCacheEntryOptions();
            if (_cacheExpiryInSeconds > 0)
                options.SetAbsoluteExpiration(TimeSpan.FromSeconds(_cacheExpiryInSeconds));
            return options;
        }

        private static IDictionary<long, Provider> LoadTempProviderData()
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            var jsonFile = $"{assemblyName}.Data.ProviderData.json";

            return JsonDocument
                .Parse(jsonFile.ReadManifestResourceStreamAsString())
                .RootElement
                .GetProperty("providers")
                .EnumerateArray()
                .Select(p =>
                    new Provider
                    {
                        UkPrn = p.GetProperty("ukPrn").GetInt64(),
                        Name = p.GetProperty("name").GetString(),
                        Locations = p.GetProperty("locations")
                            .EnumerateArray()
                            .Select(l =>
                                new Location
                                {
                                    Postcode = l.GetProperty("postcode").GetString(),
                                    Name = l.SafeGetString("name"),
                                    Town = l.SafeGetString("town"),
                                    Latitude = l.SafeGetDouble("latitude"),
                                    Longitude = l.SafeGetDouble("longitude"),
                                    Website = l.SafeGetString("website"),
                                    DeliveryYears = l.TryGetProperty("deliveryYears", out var deliveryYears)
                                        ? deliveryYears.EnumerateArray()
                                            .Select(d =>
                                                new DeliveryYearDto
                                                {
                                                    Year = d.GetProperty("year").GetInt16(),
                                                    Qualifications = d.GetProperty("qualifications")
                                                        .EnumerateArray()
                                                        .Select(q => q.GetInt32())
                                                        .ToList()
                                                })
                                            .ToList()
                                        : new List<DeliveryYearDto>()
                                }).ToList()
                    })
                .ToDictionary(p => p.UkPrn);
        }
    }
}

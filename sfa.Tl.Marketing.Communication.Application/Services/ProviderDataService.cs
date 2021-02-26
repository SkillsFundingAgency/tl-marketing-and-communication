using System;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using sfa.Tl.Marketing.Communication.Application.Extensions;
using sfa.Tl.Marketing.Communication.Models.Configuration;

namespace sfa.Tl.Marketing.Communication.Application.Services
{
    public class ProviderDataService : IProviderDataService
    {
        public const string ProviderTableDataCacheKey = "Provider_Table_Data";
        public const string QualificationTableDataCacheKey = "Qualification_Table_Data";
        private readonly int _cacheExpiryInSeconds;

        private readonly IMemoryCache _cache;
        private readonly ITableStorageService _tableStorageService;
        private readonly ILogger<ProviderDataService> _logger;

        private static readonly IDictionary<string, VenueNameOverride> VenueNameOverrides;

        static ProviderDataService()
        {
            VenueNameOverrides = GetVenueNameOverrides();
        }
        
        public ProviderDataService(
            ITableStorageService tableStorageService,
            IMemoryCache cache,
            ILogger<ProviderDataService> logger,
            ConfigurationOptions configuration)
        {
            _tableStorageService = tableStorageService ?? throw new ArgumentNullException(nameof(tableStorageService));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cacheExpiryInSeconds = configuration?.CacheExpiryInSeconds ?? 60;
        }

        public IQueryable<Provider> GetProviders()
        {
            return GetAllProviders();
        }

        public IQueryable<Location> GetLocations(IQueryable<Provider> providers, int? qualificationId = null)
        {
            return qualificationId > 0
                ? providers.SelectMany(p => p.Locations)
                    .Where(l => l.DeliveryYears.Any(d => d.Qualifications.Contains(qualificationId.Value)))
                : providers.SelectMany(p => p.Locations);
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
            var qualifications = GetAllQualifications().ToList();
            qualifications.Add(new Qualification { Id = 0, Name = "All T Level courses" });
            return qualifications;
        }

        public IEnumerable<string> GetWebsiteUrls()
        {
            var urlList = new List<string>();

            foreach (var provider in GetAllProviders())
            {
                foreach (var location in provider.Locations.Where(l => !string.IsNullOrWhiteSpace(l.Website)))
                {
                    if (!urlList.Contains(location.Website))
                    {
                        urlList.Add(location.Website);
                    }
                }
            }

            return urlList;
        }

        private IQueryable<Qualification> GetAllQualifications()
        {
            if (!_cache.TryGetValue(QualificationTableDataCacheKey,
                out IList<Qualification> qualifications))
            {
                qualifications = _tableStorageService.GetAllQualifications().GetAwaiter().GetResult();
                _cache.Set(QualificationTableDataCacheKey, qualifications, GetCacheOptions());
            }

            return qualifications.AsQueryable();
        }

        private IQueryable<Provider> GetAllProviders()
        {
            if (!_cache.TryGetValue(ProviderTableDataCacheKey,
                out IList<Provider> providers))
            {
                providers = _tableStorageService.GetAllProviders().GetAwaiter().GetResult();

                //Override location/venue names
                foreach (var provider in providers)
                {
                    foreach (var location in provider.Locations)
                    {
                        if (VenueNameOverrides
                                .TryGetValue($"{provider.UkPrn}{location.Postcode}",
                                    out var venueNameItem)
                            && location.Name != venueNameItem.VenueName)
                        {
                            _logger.LogInformation("Overriding venue name for " +
                                                   $"{provider.Name} {location.Postcode} " +
                                                   $"from {location.Name} to {venueNameItem.VenueName}");
                            location.Name = venueNameItem.VenueName;
                        }
                    }
                }
                _cache.Set(ProviderTableDataCacheKey, providers, GetCacheOptions());
            }

            return providers.AsQueryable();
        }

        private MemoryCacheEntryOptions GetCacheOptions()
        {
            var options = new MemoryCacheEntryOptions();
            if (_cacheExpiryInSeconds > 0)
                options.SetAbsoluteExpiration(TimeSpan.FromSeconds(_cacheExpiryInSeconds));
            return options;
        }

        private static IDictionary<string, VenueNameOverride> GetVenueNameOverrides()
        {
            var venueNameData = JsonSerializer
                .Deserialize<IList<VenueNameOverride>>(
                    "sfa.Tl.Marketing.Communication.Application.Data.VenueNames.json"
                        .ReadManifestResourceStreamAsString(),
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

            var venueNameOverrides = new Dictionary<string, VenueNameOverride>();
            if (venueNameData != null)
            {
                foreach (var venueName in venueNameData)
                {
                    venueNameOverrides[$"{venueName.UkPrn}{venueName.Postcode}"] = venueName;
                }
            }

            return venueNameOverrides;
        }

    }
}

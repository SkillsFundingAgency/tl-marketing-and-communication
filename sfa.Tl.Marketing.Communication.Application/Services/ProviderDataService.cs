using System;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models.Dto;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using sfa.Tl.Marketing.Communication.Models.Configuration;

namespace sfa.Tl.Marketing.Communication.Application.Services
{
    public class ProviderDataService : IProviderDataService
    {
        private const string ProviderTableDataCacheKey = "Provider_Table_Data";
        private const string QualificationTableDataCacheKey = "Qualification_Table_Data";
        private readonly int _cacheExpiryInSeconds;

        private readonly IMemoryCache _cache;
        private readonly ITableStorageService _tableStorageService;

        public ProviderDataService(
            ITableStorageService tableStorageService,
            IMemoryCache cache,
            ConfigurationOptions configuration)
        {
            _tableStorageService = tableStorageService ?? throw new ArgumentNullException(nameof(tableStorageService));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
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

        public IQueryable<ProviderLocation> GetProviderLocations(IQueryable<Location> locations, IQueryable<Provider> providers)
        {
            return locations.Select(l => new
            {
                Location = l,
                Provider = providers.Single(parent => parent.Locations.Contains(l))
            })
                .Select(pl => new ProviderLocation
                {
                    ProviderName = pl.Provider.Name,
                    Name = pl.Location.Name,
                    Latitude = pl.Location.Latitude,
                    Longitude = pl.Location.Longitude,
                    Postcode = pl.Location.Postcode,
                    Town = pl.Location.Town,
                    Website = pl.Location.Website,
                    DeliveryYears = pl.Location.DeliveryYears != null
                        ? pl.Location.DeliveryYears
                            .Select(d => new DeliveryYear
                            {
                                Year = d.Year,
                                Qualifications = d.Qualifications != null ?
                                    GetQualifications(d.Qualifications.ToArray())
                                    : new List<Qualification>()
                            })
                            .OrderBy(d => d.Year)
                            .ToList()
                        : new List<DeliveryYear>()
                })
                .Where(pl => pl.DeliveryYears.Any(y => y.Qualifications.Any()));
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
    }
}

using System;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace sfa.Tl.Marketing.Communication.Application.Services
{
    public class ProviderDataService : IProviderDataService
    {
        public const string ProviderTableDataCacheKey = "Provider_Table_Data";
        public const string QualificationTableDataCacheKey = "Qualification_Table_Data";
        public const int CacheExpiryInSeconds = 60;

        private readonly IMemoryCache _cache;
        private readonly ITableStorageService _tableStorageService;
        private readonly ILogger<ProviderDataService> _logger;

        public ProviderDataService(
            ITableStorageService tableStorageService,
            IMemoryCache cache,
            ILogger<ProviderDataService> logger)
        {
            _tableStorageService = tableStorageService ?? throw new ArgumentNullException(nameof(tableStorageService));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IQueryable<Provider> GetProviders()
        {
            return GetAllProviders();
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

        private async Task<IList<Provider>> LoadProviderTableData()
        {
            try
            {
                _logger.LogInformation("Looking for providers in table storage");
                var providersFromTable = await _tableStorageService.GetAllProviders();
                _logger.LogInformation($"Found {providersFromTable?.Count ?? 0} providers in table storage");

                return providersFromTable;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve providers from table storage");
                return null;
            }
        }

        private async Task<IList<Qualification>> LoadQualificationTableData()
        {
            try
            {
                _logger.LogInformation("Looking for qualifications in table storage");
                var qualificationsFromTable = await _tableStorageService.GetAllQualifications();
                _logger.LogInformation($"Found {qualificationsFromTable?.Count ?? 0} qualifications in table storage");

                return qualificationsFromTable;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve qualifications from table storage");
                return null;
            }
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
                out IList<Qualification> qualificationTableData))
            {
                qualificationTableData = LoadQualificationTableData().GetAwaiter().GetResult();
                _cache.Set(QualificationTableDataCacheKey,
                    qualificationTableData,
                    new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromSeconds(CacheExpiryInSeconds)));
            }

            return qualificationTableData.AsQueryable();
        }

        private IQueryable<Provider> GetAllProviders()
        {
            if (!_cache.TryGetValue(ProviderTableDataCacheKey,
                out IList<Provider> providerTableData))
            {
                providerTableData = LoadProviderTableData().GetAwaiter().GetResult();
                _cache.Set(ProviderTableDataCacheKey,
                    providerTableData,
                    new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromSeconds(CacheExpiryInSeconds)));
            }

            return providerTableData.AsQueryable();
        }
    }
}

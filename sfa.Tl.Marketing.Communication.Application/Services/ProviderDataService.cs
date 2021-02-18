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
                qualificationTableData = _tableStorageService.GetAllQualifications().GetAwaiter().GetResult();
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
                providerTableData = _tableStorageService.GetAllProviders().GetAwaiter().GetResult();
                _cache.Set(ProviderTableDataCacheKey,
                    providerTableData,
                    new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromSeconds(CacheExpiryInSeconds)));
            }

            return providerTableData.AsQueryable();
        }
    }
}

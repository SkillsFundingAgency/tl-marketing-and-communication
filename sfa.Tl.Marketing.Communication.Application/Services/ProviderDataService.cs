using System;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Extensions.Caching.Memory;
using sfa.Tl.Marketing.Communication.Application.Caching;
using sfa.Tl.Marketing.Communication.Application.Extensions;
using sfa.Tl.Marketing.Communication.Models.Configuration;

namespace sfa.Tl.Marketing.Communication.Application.Services;

public class ProviderDataService : IProviderDataService
{
    private readonly int _cacheExpiryInSeconds;
    private readonly bool _mergeTempProviderData;

    private readonly IMemoryCache _cache;
    private readonly ITableStorageService _tableStorageService;

    public ProviderDataService(
        ITableStorageService tableStorageService,
        IMemoryCache cache,
        ConfigurationOptions configuration)
    {
        _tableStorageService = tableStorageService ?? throw new ArgumentNullException(nameof(tableStorageService));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));

        if(configuration == null) throw new ArgumentNullException(nameof(configuration));
        _cacheExpiryInSeconds = configuration.CacheExpiryInSeconds;
        _mergeTempProviderData = configuration.MergeTempProviderData;
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
                                    Qualifications = (d, qualificationsDictionary).GetQualificationsForDeliveryYear()
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
        return GetAllQualifications();
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
        if (!_cache.TryGetValue(CacheKeys.QualificationTableDataKey,
                out IQueryable<Qualification> qualifications))
        {
            qualifications = _tableStorageService
                .GetAllQualifications()
                .GetAwaiter()
                .GetResult()
                .AsQueryable();
            _cache.Set(CacheKeys.QualificationTableDataKey, qualifications, CacheUtilities.DefaultMemoryCacheEntryOptions(_cacheExpiryInSeconds));
        }

        return qualifications;
    }

    private IQueryable<Provider> GetAllProviders()
    {
        if (!_cache.TryGetValue(CacheKeys.ProviderTableDataKey,
                out IQueryable<Provider> providers))
        {
            providers = _tableStorageService
                .GetAllProviders()
                .GetAwaiter()
                .GetResult()
                .MergeTempProviders(_mergeTempProviderData)
                .AsQueryable();

            _cache.Set(CacheKeys.ProviderTableDataKey, providers, CacheUtilities.DefaultMemoryCacheEntryOptions(_cacheExpiryInSeconds));
        }

        return providers;
    }
}
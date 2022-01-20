using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using sfa.Tl.Marketing.Communication.Application.Caching;
using sfa.Tl.Marketing.Communication.Application.Calculators;
using sfa.Tl.Marketing.Communication.Application.GeoLocations;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models.Configuration;
using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.Application.Services
{
    public class DistanceCalculationService : IDistanceCalculationService
    {
        private readonly ILocationApiClient _locationApiClient;
        private readonly IDateTimeService _dateTimeService;
        private readonly IMemoryCache _cache;
        private readonly int _cacheExpiryInSeconds;
        private readonly ILogger<DistanceCalculationService> _logger;

        public DistanceCalculationService(
            ILocationApiClient locationApiClient,
            IDateTimeService dateTimeService,
            IMemoryCache cache,
            ConfigurationOptions configuration,
            ILogger<DistanceCalculationService> logger)
        {
            _locationApiClient = locationApiClient ?? throw new ArgumentNullException(nameof(locationApiClient));
            _dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _cacheExpiryInSeconds = configuration?.PostcodeCacheExpiryInSeconds ?? 60;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public double CalculateDistanceInMiles(double lat1, double lon1, double lat2, double lon2)
        {
            return Haversine.Distance(lat1, lon1, lat2, lon2);
        }

        public async Task<IList<ProviderLocation>> CalculateProviderLocationDistanceInMiles(PostcodeLocation origin, IQueryable<ProviderLocation> providerLocations)
        {
            double originLatitude;
            double originLongitude;
            if (!origin.Latitude.HasValue || !origin.Longitude.HasValue)
            {
                var originGeoLocation = await _locationApiClient.GetGeoLocationDataAsync(origin.Postcode);
                originLatitude = originGeoLocation.Latitude;
                originLongitude = originGeoLocation.Longitude;
            }
            else
            {
                originLatitude = origin.Latitude.Value;
                originLongitude = origin.Longitude.Value;
            }

            var results = new List<ProviderLocation>();
            foreach (var providerLocation in providerLocations)
            {
                providerLocation.DistanceInMiles = CalculateDistanceInMiles(
                    originLatitude, originLongitude,
                    providerLocation.Latitude, providerLocation.Longitude);

                results.Add(providerLocation);
            }

            return results;
        }

        public async Task<(bool IsValid, PostcodeLocation PostcodeLocation)> IsPostcodeValid(string postcode)
        {
            var key = CacheKeys.PostcodeKey(postcode);
            if (_cache.TryGetValue(key, out PostcodeLocation postcodeLocation))
            {
                _logger.LogInformation("Cache hit - found postcode {postcode}", postcodeLocation.Postcode);
            }
            else
            {
                var location = await _locationApiClient.GetGeoLocationDataAsync(postcode);
                if (location is not null)
                {
                    postcodeLocation = new PostcodeLocation
                    {
                        Postcode = location.Postcode,
                        Latitude = location.Latitude,
                        Longitude = location.Longitude
                    };

                    if (_cacheExpiryInSeconds > 0)
                    {
                        _cache.Set(key, postcodeLocation, CacheUtilities.DefaultMemoryCacheEntryOptions(
                        _dateTimeService,
                        _logger,
                        _cacheExpiryInSeconds,
                        _cacheExpiryInSeconds
                        ));
                    }
                }
            }

            return (postcodeLocation != null, postcodeLocation);
        }
    }
}

using sfa.Tl.Marketing.Communication.Application.GeoLocations;
using sfa.Tl.Marketing.Communication.Application.GoogleMaps;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sfa.Tl.Marketing.Communication.Application.Services
{
    public class DistanceClaculationService : IDistanceCalculationService
    {
        private readonly ILocationApiClient _locationApiClient;
        private readonly IDistanceService _distanceService;

        public DistanceClaculationService(ILocationApiClient locationApiClient, IDistanceService distanceService)
        {
            _locationApiClient = locationApiClient;
            _distanceService = distanceService;
        }

        public async Task<IQueryable<ProviderLocation>> CalculateProviderLocationDistanceInMiles(string origionPostCode, IQueryable<ProviderLocation> providerLocations)
        {
            var origionGeoLocation = await _locationApiClient.GetGeoLocationDataAsync(origionPostCode, true);

            foreach (var providerLocation in providerLocations)
            {
                var distanceInMiles = _distanceService.CalculateInMiles(Convert.ToDouble(origionGeoLocation.Latitude)
                    , Convert.ToDouble(origionGeoLocation.Longitude), providerLocation.Latitude, providerLocation.Longitude);
                providerLocation.DistanceInMiles = (int)Math.Ceiling(distanceInMiles);
            }

            return providerLocations;
        }
    }
}

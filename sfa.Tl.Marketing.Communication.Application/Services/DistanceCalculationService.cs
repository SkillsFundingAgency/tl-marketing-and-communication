using sfa.Tl.Marketing.Communication.Application.GeoLocations;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sfa.Tl.Marketing.Communication.Application.Services
{
    public class DistanceCalculationService : IDistanceCalculationService
    {
        private readonly ILocationApiClient _locationApiClient;
        private readonly IDistanceService _distanceService;

        public DistanceCalculationService(ILocationApiClient locationApiClient, IDistanceService distanceService)
        {
            _locationApiClient = locationApiClient;
            _distanceService = distanceService;
        }

        public async Task<List<ProviderLocation>> CalculateProviderLocationDistanceInMiles(string origionPostCode, IQueryable<ProviderLocation> providerLocations)
        {
            var origionGeoLocation = await _locationApiClient.GetGeoLocationDataAsync(origionPostCode, true);
            var results = new List<ProviderLocation>();
            foreach (var providerLocation in providerLocations)
            {
                var distanceInMiles = _distanceService.CalculateInMiles(Convert.ToDouble(origionGeoLocation.Latitude)
                    , Convert.ToDouble(origionGeoLocation.Longitude), providerLocation.Latitude, providerLocation.Longitude);
                providerLocation.DistanceInMiles = (int)Math.Floor(distanceInMiles);
                results.Add(providerLocation);
            }

            return results;
        }

        public async Task<bool> IsPostcodeValid(string postcode)
        {
            var results = await _locationApiClient.IsValidPostcodeAsync(postcode, true);
            return results.Item1;
        }
    }
}

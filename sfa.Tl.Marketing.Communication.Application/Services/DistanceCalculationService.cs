using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using sfa.Tl.Marketing.Communication.Application.Calculators;
using sfa.Tl.Marketing.Communication.Application.GeoLocations;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.Application.Services
{
    public class DistanceCalculationService : IDistanceCalculationService
    {
        private readonly ILocationApiClient _locationApiClient;

        public DistanceCalculationService(ILocationApiClient locationApiClient)
        {
            _locationApiClient = locationApiClient ?? throw new ArgumentNullException(nameof(locationApiClient));
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
            var location = await _locationApiClient.GetGeoLocationDataAsync(postcode);
            return (location != null,
                    new PostcodeLocation
                    {
                        Postcode = location?.Postcode,
                        Latitude = location?.Latitude,
                        Longitude = location?.Longitude
                    });
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sfa.Tl.Marketing.Communication.Application.Enums;
using sfa.Tl.Marketing.Communication.Application.GeoLocations;
using sfa.Tl.Marketing.Communication.Application.Haversine;
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
            var pos1 = new Position { Latitude = lat1, Longitude = lon1 };
            var pos2 = new Position { Latitude = lat2, Longitude = lon2 };

            var distanceInMiles = Haversine.Haversine.Distance(pos1, pos2, DistanceType.Miles);
            return distanceInMiles;
        }

        public async Task<List<ProviderLocation>> CalculateProviderLocationDistanceInMiles(PostcodeLocation origin, IQueryable<ProviderLocation> providerLocations)
        {
            double latitude;
            double longitude;
            if (!origin.Latitude.HasValue || !origin.Longitude.HasValue)
            {
                var originGeoLocation = await _locationApiClient.GetGeoLocationDataAsync(origin.Postcode);
                latitude = originGeoLocation.Latitude;
                longitude = originGeoLocation.Longitude;
            }
            else
            {
                latitude = origin.Latitude.Value;
                longitude = origin.Longitude.Value;
            }

            var results = new List<ProviderLocation>();
            foreach (var providerLocation in providerLocations)
            {
                providerLocation.DistanceInMiles = CalculateDistanceInMiles(
                    latitude, longitude,
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

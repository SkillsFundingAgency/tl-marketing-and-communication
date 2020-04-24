using FluentAssertions;
using NSubstitute;
using sfa.Tl.Marketing.Communication.Application.GeoLocations;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Application.Services;
using sfa.Tl.Marketing.Communication.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace sfa.Tl.Marketing.Communication.Application.UnitTests
{
    public class DistanceCalculationServiceUnitTests
    {
        private readonly IDistanceCalculationService _service;
        private readonly ILocationApiClient _locationApiClient;
        private readonly IDistanceService _distanceService; 
        public DistanceCalculationServiceUnitTests()
        {
            _locationApiClient = Substitute.For<ILocationApiClient>();
            _distanceService = Substitute.For<IDistanceService>();
            _service = new DistanceCalculationService(_locationApiClient, _distanceService);
        }

        [Theory]
        [InlineData("mk126ab", "52.579015", "1.720474", 51.680624, -1.28696, 2.9, 2)]
        [InlineData("mk126ab", "53.579015", "2.720474", 52.680624, -2.28696, 19.3, 19)]
        [InlineData("mk126ab", "54.579015", "3.720474", 53.680624, -3.28696, 97.7, 97)]
        [InlineData("mk126ab", "55.579015", "4.720474", 54.680624, -4.28696, 100.5, 100)]
        public async Task CalculateProviderLocationDistanceInMiles_Calculate_Distance_In_Miles_From_StudentPostcode_To_ProviderLocation(string sPostcode, 
            string sLat, 
            string sLon, 
            double dLat, 
            double dLon, 
            double calculatedMilageInMiles, 
            int expectedMilageInMilesAfterRounding)
        {
            // Arrange
            bool includeTerminatedPostcode = true;
            var postcodeLookupResultDto = new PostcodeLookupResultDto() { Postcode = sPostcode, Latitude = sLat, Longitude = sLon };
            _locationApiClient.GetGeoLocationDataAsync(sPostcode, Arg.Is<bool>(a => a == includeTerminatedPostcode)).Returns(postcodeLookupResultDto);

            var providerLocation = new ProviderLocation() { Latitude = dLat, Longitude = dLon };
            var providerLocations = new List<ProviderLocation>() 
            {
               providerLocation
            };

            _distanceService.CalculateInMiles(Arg.Is<double>(a => a == Convert.ToDouble(postcodeLookupResultDto.Latitude)),
                Arg.Is<double>(a => a == Convert.ToDouble(postcodeLookupResultDto.Longitude)),
                Arg.Is<double>(a => a == providerLocation.Latitude),
                Arg.Is<double>(a => a == providerLocation.Longitude)).Returns(calculatedMilageInMiles);
            
            // Act
            var actual = await _service.CalculateProviderLocationDistanceInMiles(sPostcode, providerLocations.AsQueryable());

            // Assert
            var actualProviderLocation = actual.First();
            actualProviderLocation.DistanceInMiles.Should().Be(expectedMilageInMilesAfterRounding);
            
            await _locationApiClient.Received(1).GetGeoLocationDataAsync(sPostcode, Arg.Is<bool>(a => a == includeTerminatedPostcode));

            _distanceService.Received(1).CalculateInMiles(Arg.Is<double>(a => a == Convert.ToDouble(postcodeLookupResultDto.Latitude)),
                Arg.Is<double>(a => a == Convert.ToDouble(postcodeLookupResultDto.Longitude)),
                Arg.Is<double>(a => a == providerLocation.Latitude),
                Arg.Is<double>(a => a == providerLocation.Longitude));
        }

        [Theory]
        [InlineData("mk778gh", "ssssddddd", true)]
        [InlineData("mk665bt", "kkklk", false)]
        public async Task IsPostcodeValid_Validate_a_Postcode_Including_TerminatedPostcodes(string postcode, string data, bool isValid)
        {
            // Arrange
            bool expected = isValid;
            bool includeTerminatedPostcode = true;
            _locationApiClient.IsValidPostcodeAsync(postcode, Arg.Is<bool>(a => a == includeTerminatedPostcode)).Returns((expected, data));

            // Act
            var actual = await _service.IsPostcodeValid(postcode);

            // Assert
            actual.Should().Be(expected);
            await _locationApiClient.Received(1).IsValidPostcodeAsync(postcode, Arg.Is<bool>(a => a == includeTerminatedPostcode));
        }
    }
}

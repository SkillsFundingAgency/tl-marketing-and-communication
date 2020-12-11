using System;
using FluentAssertions;
using NSubstitute;
using sfa.Tl.Marketing.Communication.Application.GeoLocations;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Application.Services;
using sfa.Tl.Marketing.Communication.Models.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Application.Services
{
    public class DistanceCalculationServiceUnitTests
    {
        private readonly IDistanceCalculationService _distanceCalculationService;
        private readonly ILocationApiClient _locationApiClient;

        public DistanceCalculationServiceUnitTests()
        {
            _locationApiClient = Substitute.For<ILocationApiClient>();
            _distanceCalculationService = new DistanceCalculationService(_locationApiClient);
        }

        [Theory]
        [InlineData(52.05801, -0.784115, 52.133347, -0.468552, 14.375097500047632)]
        [InlineData(52.05801, -0.784115, 52.486942, -0.692251, 29.89913575084169)]
        [InlineData(52.05801, -0.784115, 53.587875, -2.294975, 123.12734511742885)]
        [InlineData(52.05801, -0.784115, 54.903545, -1.384952, 198.21372605959934)]
        public void CalculateDistanceInMiles_Calculate_Distance_In_Miles(double lat1, double lon1, double lat2, double lon2, double expected)
        {
            // Arrange
            // Act
            var actual = _distanceCalculationService.CalculateDistanceInMiles(lat1, lon1, lat2, lon2);

            // Assert
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData("mk126ab", "52.579015", "1.720474", 51.680624, -1.28696, 141)]
        [InlineData("mk126ab", "53.579015", "2.720474", 52.680624, -2.28696, 216)]
        [InlineData("mk126ab", "54.579015", "3.720474", 53.680624, -3.28696, 290)]
        [InlineData("mk126ab", "55.579015", "4.720474", 54.680624, -4.28696, 361)]
        public async Task CalculateProviderLocationDistanceInMiles_Calculate_Distance_In_Miles_From_StudentPostcode_To_ProviderLocation_Using_LocationApi(
            string studentPostcode,
            string studentLat,
            string studentLon,
            double providerLat,
            double providerLon,
            int expectedMileageInMilesAfterRounding)
        {
            // Arrange
            var postcodeLookupResultDto = new PostcodeLookupResultDto { Postcode = studentPostcode, Latitude = studentLat, Longitude = studentLon };
            _locationApiClient.GetGeoLocationDataAsync(studentPostcode).Returns(postcodeLookupResultDto);

            var providerLocation = new ProviderLocation { Latitude = providerLat, Longitude = providerLon };
            var providerLocations = new List<ProviderLocation>()
            {
               providerLocation
            };

            // Act
            var actual = await _distanceCalculationService.CalculateProviderLocationDistanceInMiles(new PostcodeLocation { Postcode = studentPostcode }, providerLocations.AsQueryable());

            // Assert
            var actualProviderLocation = actual.First();
            var roundedDistanceInMiles = (int)Math.Floor(actualProviderLocation.DistanceInMiles);
            roundedDistanceInMiles.Should().Be(expectedMileageInMilesAfterRounding);

            await _locationApiClient.Received(1).GetGeoLocationDataAsync(studentPostcode);
        }

        [Theory]
        [InlineData("mk126ab", "52.579015", "1.720474", 51.680624, -1.28696, 141)]
        [InlineData("mk126ab", "53.579015", "2.720474", 52.680624, -2.28696, 216)]
        [InlineData("mk126ab", "54.579015", "3.720474", 53.680624, -3.28696, 290)]
        [InlineData("mk126ab", "55.579015", "4.720474", 54.680624, -4.28696, 361)]
        public async Task CalculateProviderLocationDistanceInMiles_Calculate_Distance_In_Miles_From_StudentPostcode_To_ProviderLocation_Using_Saved_Lat_Long(
            string studentPostcode,
            string studentLat,
            string studentLon,
            double providerLat,
            double providerLon,
            int expectedMileageInMilesAfterRounding)
        {
            // Arrange
            var providerLocation = new ProviderLocation { Latitude = providerLat, Longitude = providerLon };
            var providerLocations = new List<ProviderLocation>()
            {
               providerLocation
            };

            // Act
            var actual = await _distanceCalculationService.CalculateProviderLocationDistanceInMiles(new PostcodeLocation
            {
                Postcode = studentPostcode,
                Latitude = studentLat,
                Longitude = studentLon
            }, providerLocations.AsQueryable());

            // Assert
            var actualProviderLocation = actual.First();
            var roundedDistanceInMiles = (int)Math.Floor(actualProviderLocation.DistanceInMiles);
            roundedDistanceInMiles.Should().Be(expectedMileageInMilesAfterRounding);
        }

        [Theory]
        [InlineData("mk778gh", true)]
        [InlineData("mk665bt", false)]
        public async Task IsPostcodeValid_Validate_a_Postcode_Including_TerminatedPostcodes(string postcode, bool isValid)
        {
            // Arrange
            var postcodeLookupResultDto = isValid
                ? new PostcodeLookupResultDto { Postcode = postcode }
                : null;

            var expected = isValid;
            _locationApiClient.GetGeoLocationDataAsync(postcode).Returns(postcodeLookupResultDto);

            // Act
            var actual = await _distanceCalculationService.IsPostcodeValid(postcode);

            // Assert
            actual.IsValid.Should().Be(expected);
            await _locationApiClient.Received(1).GetGeoLocationDataAsync(postcode);
        }
    }
}

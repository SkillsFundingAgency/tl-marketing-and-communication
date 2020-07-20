using FluentAssertions;
using NSubstitute;
using sfa.Tl.Marketing.Communication.Application.GeoLocations;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Application.Services;
using sfa.Tl.Marketing.Communication.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Application.Services
{
    public class DistanceCalculationServiceUnitTests
    {
        const double DoubleTolerance = 0.0000001;

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
        public async Task CalculateProviderLocationDistanceInMiles_Calculate_Distance_In_Miles_From_StudentPostcode_To_ProviderLocation_Using_LocationApi(
            string studentPostcode,
            string studentLat,
            string studentLon,
            double providerLat,
            double providerLon,
            double calculatedMileageInMiles,
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

            _distanceService.CalculateInMiles(
                    Arg.Is<double>(a => Math.Abs(a - Convert.ToDouble(postcodeLookupResultDto.Latitude)) < DoubleTolerance),
                    Arg.Is<double>(a => Math.Abs(a - Convert.ToDouble(postcodeLookupResultDto.Longitude)) < DoubleTolerance),
                    Arg.Is<double>(a => Math.Abs(a - providerLocation.Latitude) < DoubleTolerance),
                    Arg.Is<double>(a => Math.Abs(a - providerLocation.Longitude) < DoubleTolerance))
                .Returns(calculatedMileageInMiles);

            // Act
            var actual = await _service.CalculateProviderLocationDistanceInMiles(new PostcodeLocation { Postcode = studentPostcode }, providerLocations.AsQueryable());

            // Assert
            var actualProviderLocation = actual.First();
            actualProviderLocation.DistanceInMiles.Should().Be(expectedMileageInMilesAfterRounding);

            await _locationApiClient.Received(1).GetGeoLocationDataAsync(studentPostcode);

            _distanceService.Received(1).CalculateInMiles(
                Arg.Is<double>(a => Math.Abs(a - Convert.ToDouble(postcodeLookupResultDto.Latitude)) < DoubleTolerance),
                Arg.Is<double>(a => Math.Abs(a - Convert.ToDouble(postcodeLookupResultDto.Longitude)) < DoubleTolerance),
                Arg.Is<double>(a => Math.Abs(a - providerLocation.Latitude) < DoubleTolerance),
                Arg.Is<double>(a => Math.Abs(a - providerLocation.Longitude) < DoubleTolerance));
        }

        [Theory]
        [InlineData("mk126ab", "52.579015", "1.720474", 51.680624, -1.28696, 2.9, 2)]
        [InlineData("mk126ab", "53.579015", "2.720474", 52.680624, -2.28696, 19.3, 19)]
        [InlineData("mk126ab", "54.579015", "3.720474", 53.680624, -3.28696, 97.7, 97)]
        [InlineData("mk126ab", "55.579015", "4.720474", 54.680624, -4.28696, 100.5, 100)]
        public async Task CalculateProviderLocationDistanceInMiles_Calculate_Distance_In_Miles_From_StudentPostcode_To_ProviderLocation_Using_Saved_Lat_Long(
            string studentPostcode,
            string studentLat,
            string studentLon,
            double providerLat,
            double providerLon,
            double calculatedMileageInMiles,
            int expectedMileageInMilesAfterRounding)
        {
            // Arrange
            var providerLocation = new ProviderLocation { Latitude = providerLat, Longitude = providerLon };
            var providerLocations = new List<ProviderLocation>()
            {
               providerLocation
            };

            _distanceService.CalculateInMiles(
                    Arg.Is<double>(a => Math.Abs(a - Convert.ToDouble(studentLat)) < DoubleTolerance),
                    Arg.Is<double>(a => Math.Abs(a - Convert.ToDouble(studentLon)) < DoubleTolerance),
                    Arg.Is<double>(a => Math.Abs(a - providerLocation.Latitude) < DoubleTolerance),
                    Arg.Is<double>(a => Math.Abs(a - providerLocation.Longitude) < DoubleTolerance))
                .Returns(calculatedMileageInMiles);

            // Act
            var actual = await _service.CalculateProviderLocationDistanceInMiles(new PostcodeLocation
            {
                Postcode = studentPostcode,
                Latitude = studentLat,
                Longitude = studentLon
            }, providerLocations.AsQueryable());

            // Assert
            var actualProviderLocation = actual.First();
            actualProviderLocation.DistanceInMiles.Should().Be(expectedMileageInMilesAfterRounding);

            _distanceService.Received(1).CalculateInMiles(
                Arg.Is<double>(a => Math.Abs(a - Convert.ToDouble(studentLat)) < DoubleTolerance),
                Arg.Is<double>(a => Math.Abs(a - Convert.ToDouble(studentLon)) < DoubleTolerance),
                Arg.Is<double>(a => Math.Abs(a - providerLocation.Latitude) < DoubleTolerance),
                Arg.Is<double>(a => Math.Abs(a - providerLocation.Longitude) < DoubleTolerance));
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
            var actual = await _service.IsPostcodeValid(postcode);

            // Assert
            actual.IsValid.Should().Be(expected);
            await _locationApiClient.Received(1).GetGeoLocationDataAsync(postcode);
        }
    }
}

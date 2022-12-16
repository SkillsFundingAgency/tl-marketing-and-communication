using System;
using sfa.Tl.Marketing.Communication.Application.GeoLocations;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Application.Services;
using sfa.Tl.Marketing.Communication.Models.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using sfa.Tl.Marketing.Communication.Models.Configuration;
using sfa.Tl.Marketing.Communication.Tests.Common.Extensions;

namespace sfa.Tl.Marketing.Communication.UnitTests.Application.Services;

public class DistanceCalculationServiceUnitTests
{
    private readonly IDistanceCalculationService _distanceCalculationService;
    private readonly ILocationApiClient _locationApiClient;

    public DistanceCalculationServiceUnitTests()
    {
        _locationApiClient = Substitute.For<ILocationApiClient>();

        var dateTimeService = Substitute.For<IDateTimeService>();
        var logger = Substitute.For<ILogger<DistanceCalculationService>>();
        var cache = Substitute.For<IMemoryCache>();
        var configuration = new ConfigurationOptions
        {
            PostcodeCacheExpiryInSeconds = 120
        };

        _distanceCalculationService = new DistanceCalculationService(
            _locationApiClient, 
            dateTimeService, 
            cache, 
            configuration, 
            logger);
    }

    [Fact]
    public void DistanceCalculationService_Constructor_Guards_Against_NullParameters()
    {
        typeof(DistanceCalculationService)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Theory]
    [InlineData(52.05801, -0.784115, 52.133347, -0.468552, 14.375097500047632)]
    [InlineData(52.05801, -0.784115, 52.486942, -0.692251, 29.89913575084169)]
    [InlineData(52.05801, -0.784115, 53.587875, -2.294975, 123.12734511742885)]
    [InlineData(52.05801, -0.784115, 54.903545, -1.384952, 198.21372605959934)]
    public void CalculateDistanceInMiles_Calculate_Distance_In_Miles(double lat1, double lon1, double lat2, double lon2, double expected)
    {
        var actual = _distanceCalculationService.CalculateDistanceInMiles(lat1, lon1, lat2, lon2);

        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData("mk126ab", 52.579015, 1.720474, 51.680624, -1.28696, 142)]
    [InlineData("mk126ab", 53.579015, 2.720474, 52.680624, -2.28696, 217)]
    [InlineData("mk126ab", 54.579015, 3.720474, 53.680624, -3.28696, 290)]
    [InlineData("mk126ab", 55.579015, 4.720474, 54.680624, -4.28696, 361)]
    public async Task CalculateProviderLocationDistanceInMiles_Calculate_Distance_In_Miles_From_StudentPostcode_To_ProviderLocation_Using_LocationApi(
        string studentPostcode,
        double studentLat,
        double studentLon,
        double providerLat,
        double providerLon,
        int expectedMileageInMilesAfterRounding)
    {
        var postcodeLookupResultDto = new PostcodeLookupResultDto { Postcode = studentPostcode, Latitude = studentLat, Longitude = studentLon };
        _locationApiClient.GetGeoLocationDataAsync(studentPostcode).Returns(postcodeLookupResultDto);

        var providerLocation = new ProviderLocation { Latitude = providerLat, Longitude = providerLon };
        var providerLocations = new List<ProviderLocation>
        {
            providerLocation
        };

        await _distanceCalculationService.CalculateProviderLocationDistanceInMiles(new PostcodeLocation { Postcode = studentPostcode }, providerLocations.AsQueryable());

        var roundedDistanceInMiles = (int)Math.Round(providerLocation.DistanceInMiles, MidpointRounding.AwayFromZero);
        roundedDistanceInMiles.Should().Be(expectedMileageInMilesAfterRounding);

        await _locationApiClient.Received(1).GetGeoLocationDataAsync(studentPostcode);
    }

    [Theory]
    [InlineData("mk126ab", 52.579015, 1.720474, 51.680624, -1.28696, 142)]
    [InlineData("mk126ab", 53.579015, 2.720474, 52.680624, -2.28696, 217)]
    [InlineData("mk126ab", 54.579015, 3.720474, 53.680624, -3.28696, 290)]
    [InlineData("mk126ab", 55.579015, 4.720474, 54.680624, -4.28696, 361)]
    public async Task CalculateProviderLocationDistanceInMiles_Calculate_Distance_In_Miles_From_StudentPostcode_To_ProviderLocation_Using_Saved_Lat_Long(
        string studentPostcode,
        double studentLat,
        double studentLon,
        double providerLat,
        double providerLon,
        int expectedMileageInMilesAfterRounding)
    {
        var providerLocation = new ProviderLocation { Latitude = providerLat, Longitude = providerLon };
        var providerLocations = new List<ProviderLocation>
        {
            providerLocation
        };

        await _distanceCalculationService.CalculateProviderLocationDistanceInMiles(new PostcodeLocation
        {
            Postcode = studentPostcode,
            Latitude = studentLat,
            Longitude = studentLon
        }, providerLocations.AsQueryable());

        var roundedDistanceInMiles = (int)Math.Round(providerLocation.DistanceInMiles, MidpointRounding.AwayFromZero);
        roundedDistanceInMiles.Should().Be(expectedMileageInMilesAfterRounding);
    }

    [Theory]
    [InlineData("mk778gh", true)]
    [InlineData("mk665bt", false)]
    public async Task IsPostcodeValid_Validate_a_Postcode_Including_TerminatedPostcodes(string postcode, bool isValid)
    {
        var postcodeLookupResultDto = isValid
            ? new PostcodeLookupResultDto { Postcode = postcode }
            : null;

        var expected = isValid;
        _locationApiClient.GetGeoLocationDataAsync(postcode).Returns(postcodeLookupResultDto);

        var actual = await _distanceCalculationService.IsPostcodeValid(postcode);

        actual.IsValid.Should().Be(expected);
        await _locationApiClient.Received(1).GetGeoLocationDataAsync(postcode);
    }
}
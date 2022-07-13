using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using sfa.Tl.Marketing.Communication.Application.Extensions;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Application.Services;
using sfa.Tl.Marketing.Communication.Models.Configuration;
using sfa.Tl.Marketing.Communication.Models.Dto;
using sfa.Tl.Marketing.Communication.UnitTests.Builders;
using sfa.Tl.Marketing.Communication.UnitTests.TestHelpers;
using sfa.Tl.Marketing.Communication.UnitTests.TestHelpers.HttpClientHelpers;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Application.Services;

public class FindProviderApiDataServiceTests
{
    [Fact]
    public void FindProviderApiDataService_Constructor_Guards_Against_NullParameters()
    {
        typeof(FindProviderApiDataService)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task FindProviderApiDataService_GetProvidersJsonFromFindProviderApi_Returns_Expected_Result()
    {
        var responseJson = new FindProviderApiJsonBuilder().BuildGetAllProvidersResponse();
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        httpClientFactory
            .CreateClient(nameof(FindProviderApiDataService))
            .Returns(new TestHttpClientFactory()
                .CreateHttpClientWithBaseUri(SettingsBuilder.FindCourseApiBaseUri,
                    FindProviderApiDataService.GetProvidersEndpoint,
                    responseJson));

        var service = BuildFindProviderApiDataService(httpClientFactory);

        var result = await service.GetProvidersJsonFromFindProviderApi();

        var expectedJson = responseJson.PrettifyJsonString();
        var finalResultJson = result.PrettifyJsonString();

        finalResultJson.Should().Be(expectedJson);
    }

    [Fact]
    public async Task FindProviderApiDataService_GetQualificationsJsonFromFindProviderApi_Returns_Expected_Result()
    {
        var responseJson = new FindProviderApiJsonBuilder().BuildGetQualificationsResponse();
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        httpClientFactory
            .CreateClient(nameof(FindProviderApiDataService))
            .Returns(new TestHttpClientFactory()
                .CreateHttpClientWithBaseUri(SettingsBuilder.FindCourseApiBaseUri,
                    FindProviderApiDataService.GetQualificationsEndpoint,
                    responseJson));

        var service = BuildFindProviderApiDataService(httpClientFactory);

        var result = await service.GetQualificationsJsonFromFindProviderApi();

        var expectedJson = responseJson.PrettifyJsonString();
        var finalResultJson = result.PrettifyJsonString();

        finalResultJson.Should().Be(expectedJson);
    }

    private static FindProviderApiDataService BuildFindProviderApiDataService(
        IHttpClientFactory httpClientFactory = null,
        ITableStorageService tableStorageService = null,
        ConfigurationOptions siteConfiguration = null,
        ILogger<FindProviderApiDataService> logger = null)
    {
        httpClientFactory ??= Substitute.For<IHttpClientFactory>();
        tableStorageService ??= Substitute.For<ITableStorageService>();
        logger ??= Substitute.For<ILogger<FindProviderApiDataService>>();
        siteConfiguration ??= new ConfigurationOptions
        {
            FindProviderApiSettings = new FindProviderApiSettings
            {
                AppId = "E3A71ACE-2F9D-4119-95B8-BC8CBE58EAB9",
                ApiKey = "98E898EE-8CD4-4A3E-82B9-0E93CD176FD7",
                BaseUri = "https://fake.findproviderapi/"
            }
        };

        return new FindProviderApiDataService(httpClientFactory, tableStorageService, siteConfiguration, logger);
    }

    //TODO: Copy tests from CD version and implement
    //TODO: Consider moving validation methods to common class

    private static void ValidateProvider(Provider provider, long ukPrn, string name, int locationCount = 1)
    {
        provider.UkPrn.Should().Be(ukPrn);
        provider.Name.Should().Be(name);

        provider.Locations.Should().NotBeNull();
        provider.Locations.Should().HaveCount(locationCount);
    }

    private static void ValidateLocation(Location location, string name,
        string postcode, string town, string website,
        double latitude, double longitude,
        int deliveryYearCount = 1)
    {
        location.Name.Should().Be(name);

        location.Postcode.Should().Be(postcode);
        location.Town.Should().Be(town);
        location.Latitude.Should().Be(latitude);
        location.Longitude.Should().Be(longitude);
        location.Website.Should().Be(website);

        location.DeliveryYears.Should().NotBeNull();
        location.DeliveryYears.Should().HaveCount(deliveryYearCount);
    }

    private static void ValidateDeliveryYear(DeliveryYearDto deliveryYear, short year, IReadOnlyCollection<int> qualifications)
    {
        deliveryYear.Year.Should().Be(year);

        deliveryYear.Qualifications.Should().NotBeNull();
        deliveryYear.Qualifications.Should().HaveCount(qualifications.Count);

        foreach (var qualification in qualifications)
        {
            deliveryYear.Qualifications.Should().Contain(qualification);
        }
    }

    private static void ValidateQualification(Qualification qualification, int id, string route, string name)
    {
        qualification.Id.Should().Be(id);
        qualification.Route.Should().Be(route);
        qualification.Name.Should().Be(name);
    }
}
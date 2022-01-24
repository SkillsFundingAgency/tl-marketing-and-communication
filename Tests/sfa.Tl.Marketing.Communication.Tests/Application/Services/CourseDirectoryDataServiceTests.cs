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
using sfa.Tl.Marketing.Communication.Models.Dto;
using sfa.Tl.Marketing.Communication.UnitTests.Builders;
using sfa.Tl.Marketing.Communication.UnitTests.TestHelpers.HttpClientHelpers;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Application.Services;

public class CourseDirectoryDataServiceTests
{
    [Fact]
    public async Task CourseDirectoryDataService_GetTLevelDetailJsonFromCourseDirectoryApi_Returns_Expected_Result()
    {
        var responseJson = new CourseDirectoryJsonBuilder().BuildValidTLevelsSingleItemResponse();
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        httpClientFactory
            .CreateClient(nameof(CourseDirectoryDataService))
            .Returns(new TestHttpClientFactory()
                .CreateHttpClientWithBaseUri(SettingsBuilder.FindCourseApiBaseUri,
                    CourseDirectoryDataService.CourseDetailEndpoint,
                    responseJson));

        var service = BuildCourseDirectoryDataService(httpClientFactory);

        var result = await service.GetTLevelDetailJsonFromCourseDirectoryApi();

        var expectedJson = responseJson.PrettifyJsonString();
        var finalResultJson = result.PrettifyJsonString();

        finalResultJson.Should().Be(expectedJson);
    }

    [Fact]
    public async Task CourseDirectoryDataService_GetTLevelQualificationsJsonFromCourseDirectoryApi_Returns_Expected_Result()
    {
        var responseJson = new CourseDirectoryJsonBuilder().BuildValidTLevelDefinitionsResponse();
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        httpClientFactory
            .CreateClient(nameof(CourseDirectoryDataService))
            .Returns(new TestHttpClientFactory()
                .CreateHttpClientWithBaseUri(SettingsBuilder.FindCourseApiBaseUri,
                    CourseDirectoryDataService.QualificationsEndpoint,
                    responseJson));

        var service = BuildCourseDirectoryDataService(httpClientFactory);

        var result = await service.GetTLevelQualificationJsonFromCourseDirectoryApi();

        var expectedJson = responseJson.PrettifyJsonString();
        var finalResultJson = result.PrettifyJsonString();

        finalResultJson.Should().Be(expectedJson);
    }

    [Fact]
    public async Task CourseDirectoryDataService_ImportProviders_With_Single_Item_Returns_Expected_Result()
    {
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        httpClientFactory
            .CreateClient(nameof(CourseDirectoryDataService))
            .Returns(new TestHttpClientFactory()
                .CreateHttpClientWithBaseUri(SettingsBuilder.FindCourseApiBaseUri,
                    CourseDirectoryDataService.CourseDetailEndpoint,
                    new CourseDirectoryJsonBuilder()
                        .BuildValidTLevelsSingleItemResponse()));

        var providersPassedToSaveProviders = new List<Provider>();

        var tableStorageService = Substitute.For<ITableStorageService>();
        tableStorageService
            .SaveProviders(
                Arg.Do<IList<Provider>>(p =>
                    providersPassedToSaveProviders.AddRange(p)))
            .Returns(x => ((IList<Provider>)x[0]).Count);

        var service = BuildCourseDirectoryDataService(httpClientFactory, tableStorageService);

        var (savedCount, deletedCount) = await service
            .ImportProvidersFromCourseDirectoryApi();

        savedCount.Should().Be(1);
        deletedCount.Should().Be(0);

        providersPassedToSaveProviders.Should().HaveCount(1);

        var provider = providersPassedToSaveProviders.First();
        ValidateProvider(provider, 10000055, "ABINGDON AND WITNEY COLLEGE");

        var location = provider.Locations.First();
        ValidateLocation(location,
            "ABINGDON CAMPUS", "OX14 1GG", "Abingdon",
            "http://www.abingdon-witney.ac.uk",
            51.680637, -1.286943);

        ValidateDeliveryYear(location.DeliveryYears.First(), 2021, new[] { 36 });
    }

    [Fact]
    public async Task CourseDirectoryDataService_ImportProviders_With_Single_Item_With_Null_Location_Returns_Expected_Result()
    {
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        httpClientFactory
            .CreateClient(nameof(CourseDirectoryDataService))
            .Returns(new TestHttpClientFactory()
                .CreateHttpClientWithBaseUri(SettingsBuilder.FindCourseApiBaseUri,
                    CourseDirectoryDataService.CourseDetailEndpoint,
                    new CourseDirectoryJsonBuilder()
                        .BuildValidTLevelsSingleItemWithNullLocationTownResponse()));

        var providersPassedToSaveProviders = new List<Provider>();

        var tableStorageService = Substitute.For<ITableStorageService>();
        tableStorageService
            .SaveProviders(
                Arg.Do<IList<Provider>>(p =>
                    providersPassedToSaveProviders.AddRange(p)))
            .Returns(x => ((IList<Provider>)x[0]).Count);

        var service = BuildCourseDirectoryDataService(httpClientFactory, tableStorageService);

        var (savedCount, deletedCount) = await service
            .ImportProvidersFromCourseDirectoryApi();

        savedCount.Should().Be(1);
        deletedCount.Should().Be(0);

        providersPassedToSaveProviders.Should().HaveCount(1);

        var provider = providersPassedToSaveProviders.First();
        ValidateProvider(provider, 10000055, "ABINGDON AND WITNEY COLLEGE");

        var location = provider.Locations.First();
        ValidateLocation(location,
            "ABINGDON CAMPUS", "OX14 1GG", null,
            "http://www.abingdon-witney.ac.uk",
            51.680637, -1.286943);

        ValidateDeliveryYear(location.DeliveryYears.First(), 2021, new[] { 36 });
    }

    [Fact]
    public async Task CourseDirectoryDataService_ImportProviders_With_Multiple_Items_Deletes_One_And_Saves_No_Changes()
    {
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        httpClientFactory
            .CreateClient(nameof(CourseDirectoryDataService))
            .Returns(new TestHttpClientFactory()
                .CreateHttpClientWithBaseUri(SettingsBuilder.FindCourseApiBaseUri,
                    CourseDirectoryDataService.CourseDetailEndpoint,
                    new CourseDirectoryJsonBuilder()
                        .BuildValidTLevelsMultiItemResponse()));

        var deletedProviders = new List<Provider>();
        var savedProviders = new List<Provider>();

        var tableStorageService = Substitute.For<ITableStorageService>();
        tableStorageService
            .GetAllProviders()
            .Returns(new ProviderListBuilder().CreateKnownList().Build());
        tableStorageService
            .RemoveProviders(
                Arg.Do<IList<Provider>>(p =>
                    deletedProviders.AddRange(p)))
            .Returns(x => ((IList<Provider>)x[0]).Count);
        tableStorageService
            .SaveProviders(
                Arg.Do<IList<Provider>>(p =>
                    savedProviders.AddRange(p)))
            .Returns(x => ((IList<Provider>)x[0]).Count);

        var service = BuildCourseDirectoryDataService(httpClientFactory, tableStorageService);

        var (savedCount, deletedCount) = await service
            .ImportProvidersFromCourseDirectoryApi();

        savedCount.Should().Be(0);
        deletedCount.Should().Be(1);

        savedProviders.Should().HaveCount(0);
        deletedProviders.Should().HaveCount(1);

        ValidateProvider(deletedProviders[0], 10000001, "TEST COLLEGE TO BE DELETED");
    }

    [Fact]
    public async Task CourseDirectoryDataService_ImportProviders_With_Multiple_Items_Returns_Expected_Result()
    {
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        httpClientFactory
            .CreateClient(nameof(CourseDirectoryDataService))
            .Returns(new TestHttpClientFactory()
                .CreateHttpClientWithBaseUri(SettingsBuilder.FindCourseApiBaseUri,
                    CourseDirectoryDataService.CourseDetailEndpoint,
                    new CourseDirectoryJsonBuilder()
                        .BuildValidTLevelsMultiItemResponse()));

        var deletedProviders = new List<Provider>();
        var savedProviders = new List<Provider>();

        //Get a list of providers and change one
        var providers = new ProviderListBuilder().CreateKnownList().Build();
        providers.Single(p => p.UkPrn == 10000055)
            .Locations.First()
            .Name = "Old Venue Name";

        var tableStorageService = Substitute.For<ITableStorageService>();
        tableStorageService
            .GetAllProviders()
            .Returns(providers);
        tableStorageService
            .RemoveProviders(
                Arg.Do<IList<Provider>>(p =>
                    deletedProviders.AddRange(p)))
            .Returns(x => ((IList<Provider>)x[0]).Count);
        tableStorageService
            .SaveProviders(
                Arg.Do<IList<Provider>>(p =>
                    savedProviders.AddRange(p)))
            .Returns(x => ((IList<Provider>)x[0]).Count);

        var logger = Substitute.For<ILogger<CourseDirectoryDataService>>();
        logger.IsEnabled(LogLevel.Warning).Returns(true);

        var service = BuildCourseDirectoryDataService(httpClientFactory, tableStorageService, logger);

        var (savedCount, deletedCount) = await service
            .ImportProvidersFromCourseDirectoryApi();

        savedCount.Should().Be(1);
        deletedCount.Should().Be(1);

        savedProviders.Should().HaveCount(1);
        deletedProviders.Should().HaveCount(1);

        deletedProviders.Should().HaveCount(1);
        ValidateProvider(deletedProviders[0], 10000001, "TEST COLLEGE TO BE DELETED");

        var provider = savedProviders.OrderBy(p => p.Name).First();
        ValidateProvider(provider, 10000055, "ABINGDON AND WITNEY COLLEGE");

        provider.Locations.Should().HaveCount(1);
        var firstLocation = provider.Locations.OrderBy(l => l.Name).First();
        ValidateLocation(firstLocation,
            "ABINGDON CAMPUS", "OX14 1GG", "Abingdon",
            "http://www.abingdon-witney.ac.uk",
            51.680637, -1.286943);
        ValidateDeliveryYear(firstLocation.DeliveryYears.First(), 2021, new[] { 36 });

        const string expectedMessage =
            "Venue name for 10000055 ABINGDON AND WITNEY COLLEGE OX14 1GG changed from 'Old Venue Name' to 'ABINGDON CAMPUS'";

        logger.ReceivedCalls()
            .Where(call => (LogLevel)call.GetArguments()[0] == LogLevel.Warning)
            .Select(call => call.GetArguments())
            .Count(callArguments =>
            {
                if (callArguments.Length <= 2)
                {
                    return false;
                }

                var logLevel = (LogLevel)callArguments[0];
                var logValues = (IReadOnlyList<KeyValuePair<string, object>>)callArguments[2];

                return logLevel.Equals(LogLevel.Warning) &&
                       logValues != null &&
                       logValues.ToString()!.Equals(expectedMessage);
            })
            .Should()
            .Be(1);
    }

    [Fact]
    public async Task CourseDirectoryDataService_ImportQualifications_Returns_Expected_Result_When_No_Existing_Qualifications()
    {
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        httpClientFactory
            .CreateClient(nameof(CourseDirectoryDataService))
            .Returns(new TestHttpClientFactory()
                .CreateHttpClientWithBaseUri(SettingsBuilder.FindCourseApiBaseUri,
                    CourseDirectoryDataService.QualificationsEndpoint,
                    new CourseDirectoryJsonBuilder()
                        .BuildValidTLevelDefinitionsResponse()));

        var deletedQualifications = new List<Qualification>();
        var savedQualifications = new List<Qualification>();

        var tableStorageService = Substitute.For<ITableStorageService>();
        tableStorageService.GetAllQualifications().Returns(new List<Qualification>());
        tableStorageService
            .RemoveQualifications(
                Arg.Do<IList<Qualification>>(q =>
                    deletedQualifications.AddRange(q)))
            .Returns(x => ((IList<Qualification>)x[0]).Count);
        tableStorageService
            .SaveQualifications(
                Arg.Do<IList<Qualification>>(q =>
                    savedQualifications.AddRange(q)))
            .Returns(x => ((IList<Qualification>)x[0]).Count);

        var service = BuildCourseDirectoryDataService(httpClientFactory, tableStorageService);

        var (savedCount, deletedCount) = await service
            .ImportQualificationsFromCourseDirectoryApi();

        savedCount.Should().Be(16);
        deletedCount.Should().Be(0);

        deletedQualifications.Should().HaveCount(0);
        savedQualifications.Should().HaveCount(16);

        var orderedSavedQualifications = savedQualifications.OrderBy(q => q.Id).ToList();
        ValidateQualification(orderedSavedQualifications[0], 36, "Construction", "Design, Surveying and Planning for Construction");
        ValidateQualification(orderedSavedQualifications[1], 37, "Digital", "Digital Production, Design and Development");
        ValidateQualification(orderedSavedQualifications[2], 38, "Education", "Education and Childcare");
        ValidateQualification(orderedSavedQualifications[3], 39, "Digital", "Digital Business Services");
        ValidateQualification(orderedSavedQualifications[4], 40, "Digital", "Digital Support Services");
        ValidateQualification(orderedSavedQualifications[5], 41, "Health and Science", "Health");
        ValidateQualification(orderedSavedQualifications[6], 42, "Health and Science", "Healthcare Science");
        ValidateQualification(orderedSavedQualifications[7], 43, "Health and Science", "Science");
        ValidateQualification(orderedSavedQualifications[8], 44, "Construction", "Onsite Construction");
        ValidateQualification(orderedSavedQualifications[9], 45, "Construction", "Building Services Engineering for Construction");
        ValidateQualification(orderedSavedQualifications[10], 46, null, "Finance");
        ValidateQualification(orderedSavedQualifications[11], 47, null, "Accounting");
        ValidateQualification(orderedSavedQualifications[12], 48, null, "Design and Development for Engineering and Manufacturing");
        ValidateQualification(orderedSavedQualifications[13], 49, null, "Maintenance, Installation and Repair for Engineering and Manufacturing");
        ValidateQualification(orderedSavedQualifications[14], 50, null, "Engineering, Manufacturing, Processing and Control");
        ValidateQualification(orderedSavedQualifications[15], 51, null, "Management and Administration");
    }

    [Fact]
    public async Task CourseDirectoryDataService_ImportQualifications_Returns_Expected_Result_When_Has_Existing_Qualifications()
    {
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        httpClientFactory
            .CreateClient(nameof(CourseDirectoryDataService))
            .Returns(new TestHttpClientFactory()
                .CreateHttpClientWithBaseUri(SettingsBuilder.FindCourseApiBaseUri,
                    CourseDirectoryDataService.QualificationsEndpoint,
                    new CourseDirectoryJsonBuilder()
                        .BuildValidTLevelDefinitionsResponse()));

        var deletedQualifications = new List<Qualification>();
        var savedQualifications = new List<Qualification>();

        var tableStorageService = Substitute.For<ITableStorageService>();
        tableStorageService
            .GetAllQualifications()
            .Returns(new QualificationListBuilder().CreateKnownList().Build());
        tableStorageService
            .RemoveQualifications(
                Arg.Do<IList<Qualification>>(q =>
                    deletedQualifications.AddRange(q)))
            .Returns(x => ((IList<Qualification>)x[0]).Count);
        tableStorageService
            .SaveQualifications(
                Arg.Do<IList<Qualification>>(q =>
                    savedQualifications.AddRange(q)))
            .Returns(x => ((IList<Qualification>)x[0]).Count);

        var service = BuildCourseDirectoryDataService(httpClientFactory, tableStorageService);

        var (savedCount, deletedCount) = await service
            .ImportQualificationsFromCourseDirectoryApi();

        savedCount.Should().Be(11);
        deletedCount.Should().Be(2);

        deletedQualifications.Should().HaveCount(deletedCount);
        var orderedDeletedQualifications = deletedQualifications.OrderBy(q => q.Id).ToList();
        ValidateQualification(orderedDeletedQualifications[0], 1, "Test", "Qualification 1");
        ValidateQualification(orderedDeletedQualifications[1], 99, "None", "One to delete");

        savedQualifications.Should().HaveCount(savedCount);
        var orderedSavedQualifications = savedQualifications.OrderBy(q => q.Id).ToList();
        ValidateQualification(orderedSavedQualifications[0], 36, "Construction", "Design, Surveying and Planning for Construction");
        ValidateQualification(orderedSavedQualifications[1], 37, "Digital", "Digital Production, Design and Development");
        ValidateQualification(orderedSavedQualifications[2], 40, "Digital", "Digital Support Services");
        ValidateQualification(orderedSavedQualifications[3], 43, "Health and Science", "Science");
        ValidateQualification(orderedSavedQualifications[4], 45, "Construction", "Building Services Engineering for Construction");
        ValidateQualification(orderedSavedQualifications[5], 46, null, "Finance");
        ValidateQualification(orderedSavedQualifications[6], 47, null, "Accounting");
        ValidateQualification(orderedSavedQualifications[7], 48, null, "Design and Development for Engineering and Manufacturing");
        ValidateQualification(orderedSavedQualifications[8], 49, null, "Maintenance, Installation and Repair for Engineering and Manufacturing");
        ValidateQualification(orderedSavedQualifications[9], 50, null, "Engineering, Manufacturing, Processing and Control");
        ValidateQualification(orderedSavedQualifications[10], 51, null, "Management and Administration");
    }

    private static CourseDirectoryDataService BuildCourseDirectoryDataService(
        IHttpClientFactory httpClientFactory = null,
        ITableStorageService tableStorageService = null,
        ILogger<CourseDirectoryDataService> logger = null)
    {
        httpClientFactory ??= Substitute.For<IHttpClientFactory>();
        tableStorageService ??= Substitute.For<ITableStorageService>();
        logger ??= Substitute.For<ILogger<CourseDirectoryDataService>>();

        return new CourseDirectoryDataService(httpClientFactory, tableStorageService, logger);
    }

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
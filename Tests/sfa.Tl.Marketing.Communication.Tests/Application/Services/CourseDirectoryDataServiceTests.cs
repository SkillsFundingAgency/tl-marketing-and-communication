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
using sfa.Tl.Marketing.Communication.UnitTests.TestHelpers.HttpClient;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Application.Services
{
    public class CourseDirectoryDataServiceTests
    {
        [Fact]
        public async Task CourseDirectoryDataService_GetTLevelDetailJsonFromCourseDirectoryApi_Returns_Expected_Result()
        {
            var responseJson = new CourseDirectoryJsonBuilder().BuildValidTLevelsSingleItemResponse();
            var httpClientFactory = Substitute.For<IHttpClientFactory>();
            httpClientFactory
                .CreateClient(CourseDirectoryDataService.CourseDirectoryHttpClientName)
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
                .CreateClient(CourseDirectoryDataService.CourseDirectoryHttpClientName)
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
                .CreateClient(CourseDirectoryDataService.CourseDirectoryHttpClientName)
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
                .ImportProvidersFromCourseDirectoryApi(
                    new List<VenueNameOverride>());

            savedCount.Should().Be(1);
            deletedCount.Should().Be(0);

            providersPassedToSaveProviders.Should().HaveCount(1);

            var provider = providersPassedToSaveProviders.First();
            ValidateProvider(provider, "ABINGDON AND WITNEY COLLEGE", 10000055);

            var location = provider.Locations.First();
            ValidateLocation(location,
                "ABINGDON CAMPUS", "OX14 1GG", "Abingdon",
                "http://www.abingdon-witney.ac.uk",
                51.680637, -1.286943);

            ValidateDeliveryYear(location.DeliveryYears.First(), 2021, new[] { 36 });
        }

        [Fact]
        public async Task CourseDirectoryDataService_ImportProviders_With_Multiple_Items_Returns_Expected_Result()
        {
            var httpClientFactory = Substitute.For<IHttpClientFactory>();
            httpClientFactory
                .CreateClient(CourseDirectoryDataService.CourseDirectoryHttpClientName)
                .Returns(new TestHttpClientFactory()
                    .CreateHttpClientWithBaseUri(SettingsBuilder.FindCourseApiBaseUri,
                        CourseDirectoryDataService.CourseDetailEndpoint,
                        new CourseDirectoryJsonBuilder()
                            .BuildValidTLevelsMultiItemResponse()));

            var providersPassedToSaveProviders = new List<Provider>();

            var tableStorageService = Substitute.For<ITableStorageService>();
            tableStorageService
                .SaveProviders(
                    Arg.Do<IList<Provider>>(p =>
                        providersPassedToSaveProviders.AddRange(p)))
                .Returns(x => ((IList<Provider>)x[0]).Count);

            var service = BuildCourseDirectoryDataService(httpClientFactory, tableStorageService);

            var (savedCount, deletedCount) = await service
                .ImportProvidersFromCourseDirectoryApi(
                    new List<VenueNameOverride>());

            savedCount.Should().Be(3);
            deletedCount.Should().Be(0);

            providersPassedToSaveProviders.Should().HaveCount(3);

            var provider = providersPassedToSaveProviders.OrderBy(p => p.Name).First();
            ValidateProvider(provider, "ABINGDON AND WITNEY COLLEGE", 10000055);

            provider.Locations.Should().HaveCount(1);
            var firstLocation = provider.Locations.OrderBy(l => l.Name).First();
            ValidateLocation(firstLocation,
                "ABINGDON CAMPUS", "OX14 1GG", "Abingdon",
                "http://www.abingdon-witney.ac.uk",
                51.680637, -1.286943);
            ValidateDeliveryYear(firstLocation.DeliveryYears.First(), 2021, new[] { 36 });

            //var secondLocation = provider.Locations.OrderBy(l => l.Name).Skip(1).First();
            //ValidateLocation(secondLocation,
            //    "Witney Campus", "OX28 6NE", "Witney",
            //    "https://www.abingdon-witney.ac.uk/t-levels",
            //    51.785444, -1.487934);

            //ValidateDeliveryYear(secondLocation.DeliveryYears.First(), 2021, new[] { 37, 38, 44 });
        }

        [Fact]
        public async Task CourseDirectoryDataService_ImportQualifications_Returns_Expected_Result_When_No_Existing_Qualifications()
        {
            var httpClientFactory = Substitute.For<IHttpClientFactory>();
            httpClientFactory
                .CreateClient(CourseDirectoryDataService.CourseDirectoryHttpClientName)
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

            savedCount.Should().Be(10);
            deletedCount.Should().Be(0);

            deletedQualifications.Should().HaveCount(0);
            savedQualifications.Should().HaveCount(10);

            var orderedSavedQualifications = savedQualifications.OrderBy(q => q.Id).ToList();
            ValidateQualification(orderedSavedQualifications[0], 36, "Design, Surveying and Planning for Construction");
            ValidateQualification(orderedSavedQualifications[1], 37, "Digital Production, Design and Development");
            ValidateQualification(orderedSavedQualifications[2], 38, "Education and Childcare");
            ValidateQualification(orderedSavedQualifications[3], 39, "Digital Business Services");
            ValidateQualification(orderedSavedQualifications[4], 40, "Digital Support Services");
            ValidateQualification(orderedSavedQualifications[5], 41, "Health");
            ValidateQualification(orderedSavedQualifications[6], 42, "Healthcare Science");
            ValidateQualification(orderedSavedQualifications[7], 43, "Science");
            ValidateQualification(orderedSavedQualifications[8], 44, "Onsite Construction");
            ValidateQualification(orderedSavedQualifications[9], 45, "Building Services Engineering for Construction");
        }

        [Fact]
        public async Task CourseDirectoryDataService_ImportQualifications_Returns_Expected_Result_When_Has_Existing_Qualifications()
        {
            var httpClientFactory = Substitute.For<IHttpClientFactory>();
            httpClientFactory
                .CreateClient(CourseDirectoryDataService.CourseDirectoryHttpClientName)
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

            savedCount.Should().Be(4);
            deletedCount.Should().Be(2);

            deletedQualifications.Should().HaveCount(2);
            var orderedDeletedQualifications = deletedQualifications.OrderBy(q => q.Id).ToList();
            ValidateQualification(orderedDeletedQualifications[0], 1, "Qualification 1");
            ValidateQualification(orderedDeletedQualifications[1], 99, "One to delete");

            savedQualifications.Should().HaveCount(4);
            var orderedSavedQualifications = savedQualifications.OrderBy(q => q.Id).ToList();
            ValidateQualification(orderedSavedQualifications[0], 36, "Design, Surveying and Planning for Construction");
            ValidateQualification(orderedSavedQualifications[1], 37, "Digital Production, Design and Development");
            ValidateQualification(orderedSavedQualifications[2], 40, "Digital Support Services");
            ValidateQualification(orderedSavedQualifications[3], 43, "Science");
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

        private static void ValidateProvider(Provider provider, string name, long ukPrn, int locationCount = 1)
        {
            provider.Name.Should().Be(name);
            provider.UkPrn.Should().Be(ukPrn);

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

        private static void ValidateQualification(Qualification qualification, int id, string name)
        {
            qualification.Id.Should().Be(id);
            qualification.Name.Should().BeEquivalentTo(name);
        }
    }
}
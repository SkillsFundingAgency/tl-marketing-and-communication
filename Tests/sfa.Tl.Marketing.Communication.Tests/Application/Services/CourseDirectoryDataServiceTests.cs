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
            var responseJson = new CourseDirectoryJsonBuilder().BuildValidTLevelDetailSingleItemResponse();
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
            var responseJson = new CourseDirectoryJsonBuilder().BuildValidTLevelQualificationsResponse();
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
                            .BuildValidTLevelDetailSingleItemResponse()));

            var providersPassedToSaveProviders = new List<Provider>();

            var tableStorageService = Substitute.For<ITableStorageService>();
            tableStorageService.ClearProviders().Returns(0);
            tableStorageService
                .SaveProviders(
                    Arg.Do<IList<Provider>>(p => 
                        providersPassedToSaveProviders.AddRange(p)))
                .Returns(x => ((IList<Provider>)x[0]).Count);
            //tableStorageService.GetAllProviders().Returns(providers);
            //tableStorageService.GetAllQualifications().Returns(qualifications);

            var service = BuildCourseDirectoryDataService(httpClientFactory, tableStorageService);

            var result = await service
                .ImportProvidersFromCourseDirectoryApi(
                    new List<VenueNameOverride>());

            result.Saved.Should().Be(1);
            result.Deleted.Should().Be(0);

            providersPassedToSaveProviders.Should().HaveCount(1);

            var provider = providersPassedToSaveProviders.First();
            ValidateProvider(provider, "Demo provider", 123456);

            var location = provider.Locations.First();
            ValidateLocation(location, 
                "Provider venue", "CV1 2AA", "Coventry",
                "https://provider.com/venue/tlevel", 
                50.12345, -1.987654);

            ValidateDeliveryYear(location.DeliveryYears.First(), 2021, new[] {36});
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
                            .BuildValidTLevelDetailMultiItemResponse()));

            var providersPassedToSaveProviders = new List<Provider>();

            var tableStorageService = Substitute.For<ITableStorageService>();
            tableStorageService.ClearProviders().Returns(0);
            tableStorageService
                .SaveProviders(
                    Arg.Do<IList<Provider>>(p =>
                        providersPassedToSaveProviders.AddRange(p)))
                .Returns(x => ((IList<Provider>)x[0]).Count);
            //tableStorageService.RetrieveProviders().Returns(providers);
            //tableStorageService.RetrieveQualifications().Returns(qualifications);

            var service = BuildCourseDirectoryDataService(httpClientFactory, tableStorageService);

            var result = await service
                .ImportProvidersFromCourseDirectoryApi(
                    new List<VenueNameOverride>());

            result.Saved.Should().Be(3);
            result.Deleted.Should().Be(0);

            providersPassedToSaveProviders.Should().HaveCount(3);

            var provider = providersPassedToSaveProviders.First();
            provider.Name.Should().Be("Abingdon and Witney College");
            provider.UkPrn.Should().Be(10000055);

            provider.Locations.Should().NotBeNull();
            provider.Locations.Should().HaveCount(2);

            ValidateProvider(provider, "Abingdon and Witney College", 10000055, 2);

            var firstLocation = provider.Locations.First();
            ValidateLocation(firstLocation,
                "Abingdon Campus", "OX14 1GG", "Abingdon",
                "https://www.abingdon-witney.ac.uk/t-levels",
                51.680624, -1.28696);

            ValidateDeliveryYear(firstLocation.DeliveryYears.First(), 2021, new[] { 37, 38, 44 });

            var secondLocation = provider.Locations.Skip(1).First();
            ValidateLocation(secondLocation,
                "Witney Campus", "OX28 6NE", "Witney",
                "https://www.abingdon-witney.ac.uk/t-levels",
                51.785444, -1.487934);

            ValidateDeliveryYear(secondLocation.DeliveryYears.First(), 2021, new[] { 37, 38, 44 });
        }

        [Fact]
        public async Task CourseDirectoryDataService_ImportQualifications_Returns_Expected_Result()
        {
            var httpClientFactory = Substitute.For<IHttpClientFactory>();
            httpClientFactory
                .CreateClient(CourseDirectoryDataService.CourseDirectoryHttpClientName)
                .Returns(new TestHttpClientFactory()
                    .CreateHttpClientWithBaseUri(SettingsBuilder.FindCourseApiBaseUri,
                        CourseDirectoryDataService.QualificationsEndpoint,
                        new CourseDirectoryJsonBuilder()
                            .BuildValidTLevelQualificationsResponse()));

            var qualificationsPassedToSaveQualifications = new List<Qualification>();

            var tableStorageService = Substitute.For<ITableStorageService>();
            tableStorageService.ClearQualifications().Returns(0);
            tableStorageService
                .SaveQualifications(
                    Arg.Do<IList<Qualification>>(q =>
                        qualificationsPassedToSaveQualifications.AddRange(q)))
                .Returns(x => ((IList<Qualification>)x[0]).Count);

            var service = BuildCourseDirectoryDataService(httpClientFactory, tableStorageService);

            var result = await service
                .ImportQualificationsFromCourseDirectoryApi();

            result.Saved.Should().Be(10);
            result.Deleted.Should().Be(0);

            qualificationsPassedToSaveQualifications.Should().HaveCount(10);

            var firstQualification = qualificationsPassedToSaveQualifications
                .OrderBy(q => q.Id)
                .First();
            firstQualification.Id.Should().Be(36);
            firstQualification.Name.Should().Be("Design, Surveying and Planning for Construction");

            var lastQualification = qualificationsPassedToSaveQualifications
                .OrderBy(q => q.Id)
                .Last();
            lastQualification.Id.Should().Be(45);
            lastQualification.Name.Should().Be("Healthcare Science");
        }
        
        private CourseDirectoryDataService BuildCourseDirectoryDataService(
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
    }
}
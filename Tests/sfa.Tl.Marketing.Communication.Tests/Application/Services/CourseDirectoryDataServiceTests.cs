using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.Core;
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
        public async Task CourseDirectoryDataService_GetJsonFromCourseDirectoryApi_Returns_Expected_Result()
        {
            var responseJson = new CourseDirectoryJsonBuilder().BuildValidTLevelDetailResponse();
            var httpClientFactory = Substitute.For<IHttpClientFactory>();
            httpClientFactory
                .CreateClient(CourseDirectoryDataService.CourseDirectoryHttpClientName)
                .Returns(new TestHttpClientFactory()
                    // ReSharper disable once StringLiteralTypo
                    .CreateHttpClientWithBaseUri(SettingsBuilder.FindCourseApiBaseUri, "tleveldetail", responseJson));

            var service = BuildCourseDirectoryDataService(httpClientFactory);

            var result = await service.GetJsonFromCourseDirectoryApi();

            var expectedJson = responseJson.PrettifyJsonString();
            var finalResultJson = result.PrettifyJsonString();

            finalResultJson.Should().Be(expectedJson);
        }

        [Fact]
        public async Task CourseDirectoryDataService_Import_Returns_Expected_Result()
        {
            var responseJson = new CourseDirectoryJsonBuilder().BuildValidTLevelDetailResponse();
            var httpClientFactory = Substitute.For<IHttpClientFactory>();
            httpClientFactory
                .CreateClient(CourseDirectoryDataService.CourseDirectoryHttpClientName)
                .Returns(new TestHttpClientFactory()
                    // ReSharper disable once StringLiteralTypo
                    .CreateHttpClientWithBaseUri(SettingsBuilder.FindCourseApiBaseUri, "tleveldetail", responseJson));

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

            var result = await service.ImportFromCourseDirectoryApi();

            result.Should().Be(1);

            providersPassedToSaveProviders.Should().HaveCount(1);

            var provider = providersPassedToSaveProviders.First();
            provider.Name.Should().Be("Demo provider");
            provider.UkPrn.Should().Be(123456);

            provider.Locations.Should().NotBeNull();
            provider.Locations.Should().HaveCount(1);

            var location = provider.Locations.First();
            location.Name.Should().Be("Provider venue");
            location.Postcode.Should().Be("CV1 2AA");
        }

        [Fact]
        public async Task CourseDirectoryDataService_GetProviders_Returns_Expected_Result()
        {
            var providers = new ProviderListBuilder()
                .Add(2)
                .Build();
            var tableStorageService = Substitute.For<ITableStorageService>();
            tableStorageService.RetrieveProviders().Returns(providers);

            var service = BuildCourseDirectoryDataService(tableStorageService: tableStorageService);

            var result = await service.GetProviders();

            result.Should().BeEquivalentTo(providers);
        }

        [Fact]
        public async Task CourseDirectoryDataService_GetQualifications_Returns_Expected_Result()
        {
            var qualifications = new QualificationListBuilder()
                .Add(2)
                .Build();

            var tableStorageService = Substitute.For<ITableStorageService>();
            tableStorageService.RetrieveQualifications().Returns(qualifications);

            var service = BuildCourseDirectoryDataService(tableStorageService: tableStorageService);

            var result = await service.GetQualifications();

            result.Should().BeEquivalentTo(qualifications);
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
    }
}
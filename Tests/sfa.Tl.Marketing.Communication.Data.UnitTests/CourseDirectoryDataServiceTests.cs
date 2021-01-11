using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using sfa.Tl.Marketing.Communication.Data.Services;
using sfa.Tl.Marketing.Communication.Data.UnitTests.Builders;
using sfa.Tl.Marketing.Communication.Data.UnitTests.TestHelpers.HttpClient;
using Xunit;

namespace sfa.Tl.Marketing.Communication.Data.UnitTests
{
    public class CourseDirectoryDataServiceTests
    {
        [Fact]
        public async Task CourseDirectoryDataService_Import_Returns_Expected_Result()
        {
            var responseJson = new CourseDirectoryJsonBuilder().BuildValidTLevelDetailResponse();
            var logger = Substitute.For<ILogger<CourseDirectoryDataService>>();

            var httpClientFactory = Substitute.For<IHttpClientFactory>();
            httpClientFactory
                .CreateClient(CourseDirectoryDataService.CourseDirectoryHttpClientName)
                .Returns(new TestHttpClientFactory()
                    // ReSharper disable once StringLiteralTypo
                    .CreateHttpClientWithBaseUri(SettingsBuilder.FindCourseApiBaseUri, "tleveldetail", responseJson));

            var service = new CourseDirectoryDataService(httpClientFactory, logger);

            var result = await service.ImportFromCourseDirectoryApi();

            result.Should().Be(1);

            //var expectedJson = responseJson.PrettifyJsonString();
            //var finalResultJson = result.PrettifyJsonString();

            //finalResultJson.Should().Be(expectedJson);
        }
    }
}
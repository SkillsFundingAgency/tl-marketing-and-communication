using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Timers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using sfa.Tl.Marketing.Communication.Application.Extensions;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Functions.UnitTests.Builders;
using Xunit;

namespace sfa.Tl.Marketing.Communication.Functions.UnitTests
{
    public class CourseDirectoryImportFunctionTests
    {
        [Fact]
        public async Task CourseDirectoryImportFunction_Scheduled_Import_Calls_CourseDirectoryDataService_Import()
        {
            var service = Substitute.For<ICourseDirectoryDataService>();
            service.ImportFromCourseDirectoryApi().Returns(10);

            var timerSchedule = Substitute.For<TimerSchedule>();
            var logger = new NullLogger<CourseDirectoryImportFunctions>();

            var functions = new CourseDirectoryImportFunctions(service);
            await functions.ImportCourseDirectoryData(
                new TimerInfo(timerSchedule, new ScheduleStatus()),
                new ExecutionContext(),
                logger);

            await service.Received(1).ImportFromCourseDirectoryApi();
        }

        [Fact]
        public async Task CourseDirectoryImportFunction_ManualImport_Calls_CourseDirectoryDataService_Import()
        {
            var service = Substitute.For<ICourseDirectoryDataService>();
            service.ImportFromCourseDirectoryApi().Returns(10);

            var request = BuildHttpRequest(HttpMethod.Get);

            var logger = Substitute.For<ILogger>();

            var functions = new CourseDirectoryImportFunctions(service);
            var result = await functions.ManualImport(request, logger);

            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().Be("10 records saved.");

            await service.Received(1).ImportFromCourseDirectoryApi();
        }

        [Fact]
        public async Task CourseDirectoryImportFunction_GetProviders_Returns_Expected_Result()
        {
            var builder = new ProviderBuilder();
            var providers = builder.BuildList();
            var expectedResult = builder.BuildJson().PrettifyJsonString();

            var service = Substitute.For<ICourseDirectoryDataService>();
            service.GetProviders().Returns(providers);

            var request = BuildHttpRequest(HttpMethod.Get);

            var logger = Substitute.For<ILogger>();

            var functions = new CourseDirectoryImportFunctions(service);
            var result = await functions.GetProviders(request, logger);

            var jsonResult = result as JsonResult;
            jsonResult.Should().NotBeNull();

            var json = JsonSerializer.Serialize(jsonResult?.Value,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
            json.PrettifyJsonString().Should().Be(expectedResult);
        }

        [Fact]
        public async Task CourseDirectoryImportFunction_GetQualifications_Returns_Expected_Result()
        {
            var builder = new QualificationBuilder();
            var qualifications = builder.BuildList();
            var expectedResult = builder.BuildJson().PrettifyJsonString();

            var service = Substitute.For<ICourseDirectoryDataService>();
            service.GetQualifications().Returns(qualifications);

            var request = BuildHttpRequest(HttpMethod.Get);

            var logger = Substitute.For<ILogger>();

            var functions = new CourseDirectoryImportFunctions(service);
            var result = await functions.GetQualifications(request, logger);
            
            var jsonResult = result as JsonResult;
            jsonResult.Should().NotBeNull();

            var json = JsonSerializer.Serialize(jsonResult?.Value,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
            json.PrettifyJsonString().Should().Be(expectedResult);
        }

        private static HttpRequest BuildHttpRequest(HttpMethod method)
        {
            var httpContext = new DefaultHttpContext();
            var request = httpContext.Request;
            request.Method = method.ToString();

            return request;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web.Http;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Timers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using sfa.Tl.Marketing.Communication.Application.Extensions;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Functions.UnitTests.Builders;
using sfa.Tl.Marketing.Communication.Models.Dto;
using Xunit;

namespace sfa.Tl.Marketing.Communication.Functions.UnitTests
{
    public class CourseDirectoryImportFunctionTests
    {
        [Fact]
        public async Task CourseDirectoryImportFunction_Scheduled_Import_Calls_CourseDirectoryDataService_Import_Methods()
        {
            var service = Substitute.For<ICourseDirectoryDataService>();
            service
                .ImportProvidersFromCourseDirectoryApi(Arg.Any<IList<VenueNameOverride>>())
                .Returns(10);
            service
                .ImportQualificationsFromCourseDirectoryApi()
                .Returns(12);

            var timerSchedule = Substitute.For<TimerSchedule>();
            var logger = new NullLogger<CourseDirectoryImportFunctions>();

            var functions = new CourseDirectoryImportFunctions(service);
            await functions.ImportCourseDirectoryData(
                new TimerInfo(timerSchedule, new ScheduleStatus()),
                new ExecutionContext(),
                logger);

            await service.Received(1).ImportQualificationsFromCourseDirectoryApi();
            await service.Received(1).ImportProvidersFromCourseDirectoryApi(Arg.Any<IList<VenueNameOverride>>());
        }

        [Fact]
        public async Task CourseDirectoryImportFunction_ManualImport_Returns_Expected_Result()
        {
            var service = Substitute.For<ICourseDirectoryDataService>();
            service
                .ImportProvidersFromCourseDirectoryApi(Arg.Any<IList<VenueNameOverride>>())
                .Returns(10);
            service
                .ImportQualificationsFromCourseDirectoryApi()
                .Returns(12);

            var request = BuildHttpRequest(HttpMethod.Get);

            var logger = Substitute.For<ILogger>();

            var functions = new CourseDirectoryImportFunctions(service);
            var result = await functions.ManualImport(request, logger);

            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().Be("10 records saved.");
        }

        [Fact]
        public async Task CourseDirectoryImportFunction_ManualImport_Exception_Returns_Expected_Result()
        {
            var service = Substitute.For<ICourseDirectoryDataService>();
            service
                .ImportProvidersFromCourseDirectoryApi(Arg.Any<IList<VenueNameOverride>>())
                .ThrowsForAnyArgs(new InvalidOperationException());

            var request = BuildHttpRequest(HttpMethod.Get);

            var logger = Substitute.For<ILogger>();

            var functions = new CourseDirectoryImportFunctions(service);
            var result = await functions.ManualImport(request, logger);
            result.Should().BeOfType<InternalServerErrorResult>();
        }

        [Fact]
        public async Task CourseDirectoryImportFunction_GetCourseDirectoryDetailJson_Returns_Expected_Result()
        {
            const string expectedJson = "{ \"detail\": \"result\" } ";

            var service = Substitute.For<ICourseDirectoryDataService>();
            service
                .GetTLevelDetailJsonFromCourseDirectoryApi()
                .Returns(expectedJson);

            var request = BuildHttpRequest(HttpMethod.Get);

            var logger = Substitute.For<ILogger>();

            var functions = new CourseDirectoryImportFunctions(service);
            var result = await functions.GetCourseDirectoryDetailJson(request, logger);

            result.Should().BeOfType<ContentResult>();
            var contentResult = result as ContentResult;
            contentResult?.ContentType.Should().Be("application/json");
            contentResult?.Content.Should().Be(expectedJson);
        }

        [Fact]
        public async Task CourseDirectoryImportFunction_GetCourseDirectoryQualificationJson_Returns_Expected_Result()
        {
            const string expectedJson = "{ \"qualification\": \"result\" } ";

            var service = Substitute.For<ICourseDirectoryDataService>();
            service
                .GetTLevelQualificationJsonFromCourseDirectoryApi()
                .Returns(expectedJson);

            var request = BuildHttpRequest(HttpMethod.Get);

            var logger = Substitute.For<ILogger>();

            var functions = new CourseDirectoryImportFunctions(service);
            var result = await functions.GetCourseDirectoryQualificationJson(request, logger);

            result.Should().BeOfType<ContentResult>();
            var contentResult = result as ContentResult;
            contentResult?.ContentType.Should().Be("application/json");
            contentResult?.Content.Should().Be(expectedJson);
        }

        [Fact]
        public async Task CourseDirectoryImportFunction_GetCourseDirectoryDetailJson_Exception_Returns_Expected_Result()
        {
            var service = Substitute.For<ICourseDirectoryDataService>();
            service
                .GetTLevelDetailJsonFromCourseDirectoryApi()
                .ThrowsForAnyArgs(new InvalidOperationException());

            var request = BuildHttpRequest(HttpMethod.Get);

            var logger = Substitute.For<ILogger>();

            var functions = new CourseDirectoryImportFunctions(service);
            var result = await functions.GetCourseDirectoryDetailJson(request, logger);
            result.Should().BeOfType<InternalServerErrorResult>();
        }

        [Fact]
        public async Task CourseDirectoryImportFunction_GetCourseDirectoryQualificationJson_Exception_Returns_Expected_Result()
        {
            var service = Substitute.For<ICourseDirectoryDataService>();
            service
                .GetTLevelQualificationJsonFromCourseDirectoryApi()
                .ThrowsForAnyArgs(new InvalidOperationException());

            var request = BuildHttpRequest(HttpMethod.Get);

            var logger = Substitute.For<ILogger>();

            var functions = new CourseDirectoryImportFunctions(service);
            var result = await functions.GetCourseDirectoryQualificationJson(request, logger);
            result.Should().BeOfType<InternalServerErrorResult>();
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
        public async Task CourseDirectoryImportFunction_GetProviders_Exception_Returns_Expected_Result()
        {
            var service = Substitute.For<ICourseDirectoryDataService>();
            service
                .GetProviders()
                .ThrowsForAnyArgs(new InvalidOperationException());

            var request = BuildHttpRequest(HttpMethod.Get);

            var logger = Substitute.For<ILogger>();

            var functions = new CourseDirectoryImportFunctions(service);
            var result = await functions.GetProviders(request, logger);

            result.Should().BeOfType<InternalServerErrorResult>();
        }

        [Fact]
        public async Task CourseDirectoryImportFunction_GetQualifications_Returns_Expected_Result()
        {
            var builder = new QualificationBuilder();
            var qualifications = builder.BuildList();
            var expectedResult = builder.BuildJson().PrettifyJsonString();

            var service = Substitute.For<ICourseDirectoryDataService>();
            service
                .GetQualifications()
                .Returns(qualifications);

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

        [Fact]
        public async Task CourseDirectoryImportFunction_GetQualifications_Exception_Returns_Expected_Result()
        {
            var service = Substitute.For<ICourseDirectoryDataService>();
            service
                .GetQualifications()
                .ThrowsForAnyArgs(new InvalidOperationException());

            var request = BuildHttpRequest(HttpMethod.Get);

            var logger = Substitute.For<ILogger>();

            var functions = new CourseDirectoryImportFunctions(service);
            var result = await functions.GetQualifications(request, logger);

            result.Should().BeOfType<InternalServerErrorResult>();
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

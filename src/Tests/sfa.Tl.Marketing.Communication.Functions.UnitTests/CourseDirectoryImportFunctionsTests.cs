using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using NSubstitute.ExceptionExtensions;
using sfa.Tl.Marketing.Communication.Application.Extensions;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Functions.UnitTests.Builders;
using sfa.Tl.Marketing.Communication.Functions.UnitTests.Extensions;
using sfa.Tl.Marketing.Communication.Tests.Common.Extensions;

namespace sfa.Tl.Marketing.Communication.Functions.UnitTests;

public class CourseDirectoryImportFunctionsTests
{
    [Fact]
    public void CourseDirectoryImportFunctions_Constructor_Guards_Against_NullParameters()
    {
        typeof(CourseDirectoryImportFunctions)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task CourseDirectoryImportFunctions_Scheduled_Import_Calls_CourseDirectoryDataService_Import_Methods()
    {
        var service = Substitute.For<ICourseDirectoryDataService>();
        service
            .ImportProvidersFromCourseDirectoryApi()
            .Returns((10, 0));
        service
            .ImportQualificationsFromCourseDirectoryApi()
            .Returns((12, 0));

        var functionContext = FunctionObjectsBuilder.BuildFunctionContext();
        var functions = CourseDirectoryImportFunctionsBuilder.Build(service);
        await functions.ImportCourseDirectoryData(
            new TimerInfo(),
            functionContext);

        await service.Received(1).ImportQualificationsFromCourseDirectoryApi();
        await service.Received(1).ImportProvidersFromCourseDirectoryApi();
    }

    [Fact]
    public async Task CourseDirectoryImportFunctions_Scheduled_Import_Logs_Expected_Messages()
    {
        var service = Substitute.For<ICourseDirectoryDataService>();
        service
            .ImportProvidersFromCourseDirectoryApi()
            .Returns((10, 4));
        service
            .ImportQualificationsFromCourseDirectoryApi()
            .Returns((12, 2));

        var logger = Substitute.For<ILogger<object>>();

        var functionContext = FunctionObjectsBuilder.BuildFunctionContext(logger);
        var functions = CourseDirectoryImportFunctionsBuilder.Build(service);
        await functions.ImportCourseDirectoryData(
            new TimerInfo(),
            functionContext);

        logger.ReceivedCalls().Count().Should().Be(4);

        logger.HasLoggedMessage("Course directory scheduled import function was called.");

        logger.HasLoggedMessage("Course directory import inserted or updated 10 and deleted 4 providers.");
        logger.HasLoggedMessage("Course directory import inserted or updated 12 and deleted 2 qualifications.");

        logger.HasLoggedMessage("Course directory scheduled import finished.");
    }

    [Fact]
    public async Task CourseDirectoryImportFunctions_Scheduled_Import_Exception_Logs_Expected_Messages()
    {
        var service = Substitute.For<ICourseDirectoryDataService>();
        service
            .ImportProvidersFromCourseDirectoryApi()
            .ThrowsForAnyArgs(new InvalidOperationException());

        var logger = Substitute.For<ILogger<object>>();

        var functionContext = FunctionObjectsBuilder.BuildFunctionContext(logger);
        var functions = CourseDirectoryImportFunctionsBuilder.Build(service);
        await functions.ImportCourseDirectoryData(
            new TimerInfo(),
            functionContext);

        logger.ReceivedCalls().Count().Should().BeGreaterThan(0);
        logger.HasLoggedMessageLike("Error importing data from course directory. " +
                                    "Internal Error Message System.InvalidOperationException",
            LogLevel.Error);
    }

    [Fact]
    public async Task CourseDirectoryImportFunctions_GetCourseDirectoryDetailJson_Returns_Expected_Result()
    {
        const string expectedJson = "{ \"detail\": \"result\" } ";

        var service = Substitute.For<ICourseDirectoryDataService>();
        service
            .GetTLevelDetailJsonFromCourseDirectoryApi()
            .Returns(expectedJson);

        var functionContext = FunctionObjectsBuilder.BuildFunctionContext();
        var request = FunctionObjectsBuilder.BuildHttpRequestData(HttpMethod.Get);

        var functions = CourseDirectoryImportFunctionsBuilder.Build(service);
        var result = await functions.GetCourseDirectoryDetailJson(request, functionContext);

        result.Headers.GetValues("Content-Type").Should().NotBeNull();
        result.Headers.GetValues("Content-Type").First().Should().Be("application/json");

        var body = await result.Body.ReadAsString();
        body.Should().Be(expectedJson);
    }

    [Fact]
    public async Task CourseDirectoryImportFunctions_GetCourseDirectoryQualificationJson_Returns_Expected_Result()
    {
        const string expectedJson = "{ \"qualification\": \"result\" } ";

        var service = Substitute.For<ICourseDirectoryDataService>();
        service
            .GetTLevelQualificationJsonFromCourseDirectoryApi()
            .Returns(expectedJson);

        var functionContext = FunctionObjectsBuilder.BuildFunctionContext();
        var request = FunctionObjectsBuilder.BuildHttpRequestData(HttpMethod.Get);

        var functions = CourseDirectoryImportFunctionsBuilder.Build(service);
        var result = await functions.GetCourseDirectoryQualificationJson(request, functionContext);

        result.Headers.GetValues("Content-Type").Should().NotBeNull();
        result.Headers.GetValues("Content-Type").First().Should().Be("application/json");

        var body = await result.Body.ReadAsString();
        body.Should().Be(expectedJson);
    }

    [Fact]
    public async Task CourseDirectoryImportFunctions_GetCourseDirectoryDetailJson_Exception_Returns_Expected_Result()
    {
        var service = Substitute.For<ICourseDirectoryDataService>();
        service
            .GetTLevelDetailJsonFromCourseDirectoryApi()
            .ThrowsForAnyArgs(new InvalidOperationException());

        var functionContext = FunctionObjectsBuilder.BuildFunctionContext();
        var request = FunctionObjectsBuilder.BuildHttpRequestData(HttpMethod.Get);

        var functions = CourseDirectoryImportFunctionsBuilder.Build(service);
        var result = await functions.GetCourseDirectoryDetailJson(request, functionContext);

        result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task CourseDirectoryImportFunctions_GetCourseDirectoryQualificationJson_Exception_Returns_Expected_Result()
    {
        var service = Substitute.For<ICourseDirectoryDataService>();
        service
            .GetTLevelQualificationJsonFromCourseDirectoryApi()
            .ThrowsForAnyArgs(new InvalidOperationException());

        var functionContext = FunctionObjectsBuilder.BuildFunctionContext();
        var request = FunctionObjectsBuilder.BuildHttpRequestData(HttpMethod.Get);

        var functions = CourseDirectoryImportFunctionsBuilder.Build(service);
        var result = await functions.GetCourseDirectoryQualificationJson(request, functionContext);

        result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task CourseDirectoryImportFunctions_GetProviders_Returns_Expected_Result()
    {
        var builder = new ProviderBuilder();
        var providers = builder.BuildList();
        var expectedResult = builder.BuildJson().PrettifyJsonString();

        var tableStorageService = Substitute.For<ITableStorageService>();
        tableStorageService.GetAllProviders().Returns(providers);

        var functionContext = FunctionObjectsBuilder.BuildFunctionContext();
        var request = FunctionObjectsBuilder.BuildHttpRequestData(HttpMethod.Get);

        var functions = CourseDirectoryImportFunctionsBuilder.Build(tableStorageService: tableStorageService);
        var result = await functions.GetProviders(request, functionContext);

        result.Headers.GetValues("Content-Type").Should().NotBeNull();
        result.Headers.GetValues("Content-Type").First().Should().Be("application/json");

        var json = await result.Body.ReadAsString();

        json.PrettifyJsonString().Should().Be(expectedResult);
    }

    [Fact]
    public async Task CourseDirectoryImportFunctions_GetProviders_Exception_Returns_Expected_Result()
    {
        var tableStorageService = Substitute.For<ITableStorageService>();
        tableStorageService
            .GetAllProviders()
            .ThrowsForAnyArgs(new InvalidOperationException());

        var functionContext = FunctionObjectsBuilder.BuildFunctionContext();
        var request = FunctionObjectsBuilder.BuildHttpRequestData(HttpMethod.Get);

        var functions = CourseDirectoryImportFunctionsBuilder.Build(tableStorageService: tableStorageService);
        var result = await functions.GetProviders(request, functionContext);

        result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task CourseDirectoryImportFunctions_GetQualifications_Returns_Expected_Result()
    {
        var builder = new QualificationBuilder();
        var qualifications = builder.BuildList();
        var expectedResult = builder.BuildJson().PrettifyJsonString();

        var tableStorageService = Substitute.For<ITableStorageService>();
        tableStorageService.GetAllQualifications().Returns(qualifications);

        var functionContext = FunctionObjectsBuilder.BuildFunctionContext();
        var request = FunctionObjectsBuilder.BuildHttpRequestData(HttpMethod.Get);

        var functions = CourseDirectoryImportFunctionsBuilder.Build(tableStorageService: tableStorageService);
        var result = await functions.GetQualifications(request, functionContext);

        result.Headers.GetValues("Content-Type").Should().NotBeNull();
        result.Headers.GetValues("Content-Type").First().Should().Be("application/json");

        var json = await result.Body.ReadAsString();
        json.PrettifyJsonString().Should().Be(expectedResult);
    }

    [Fact]
    public async Task CourseDirectoryImportFunctions_GetQualifications_Exception_Returns_Expected_Result()
    {
        var tableStorageService = Substitute.For<ITableStorageService>();
        tableStorageService
            .GetAllQualifications()
            .ThrowsForAnyArgs(new InvalidOperationException());

        var functionContext = FunctionObjectsBuilder.BuildFunctionContext();
        var request = FunctionObjectsBuilder.BuildHttpRequestData(HttpMethod.Get);

        var functions = CourseDirectoryImportFunctionsBuilder.Build(tableStorageService: tableStorageService);
        var result = await functions.GetQualifications(request, functionContext);

        result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }
}
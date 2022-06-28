using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using sfa.Tl.Marketing.Communication.Application.Extensions;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Functions.UnitTests.Builders;
using sfa.Tl.Marketing.Communication.Functions.UnitTests.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace sfa.Tl.Marketing.Communication.Functions.UnitTests;

public class CourseDirectoryImportFunctionTests //: IClassFixture<FunctionTestFixture>
{
    // ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly ILogger _logger;
    private readonly ILoggerFactory _loggerFactory;
    // ReSharper restore PrivateFieldCanBeConvertedToLocalVariable
    private readonly FunctionContext _functionContext;
    //private readonly FunctionTestFixture _fixture;

    public CourseDirectoryImportFunctionTests(
        //FunctionTestFixture fixture,
        ITestOutputHelper testOutputHelper)
    {
        //_fixture = fixture;
        _testOutputHelper = testOutputHelper;

        _logger = Substitute.For<ILogger>();
        _loggerFactory = Substitute.For<ILoggerFactory>();
        _loggerFactory.CreateLogger(Arg.Any<string>())
            .Returns(_logger);

        _functionContext = Substitute.For<FunctionContext>();
        _functionContext.InstanceServices.GetService(Arg.Any<Type>())
            .Returns(_loggerFactory);

        _testOutputHelper.WriteLine("Test initialised");
    }

    [Fact]
    public async Task CourseDirectoryImportFunction_Scheduled_Import_Calls_CourseDirectoryDataService_Import_Methods()
    {
        var service = Substitute.For<ICourseDirectoryDataService>();
        service
            .ImportProvidersFromCourseDirectoryApi()
            .Returns((10, 0));
        service
            .ImportQualificationsFromCourseDirectoryApi()
            .Returns((12, 0));

        var functions = new CourseDirectoryImportFunctionsBuilder().BuildCourseDirectoryImportFunctions(service);
        await functions.ImportCourseDirectoryData(
            new TimerInfo(),
            _functionContext);

        await service.Received(1).ImportQualificationsFromCourseDirectoryApi();
        await service.Received(1).ImportProvidersFromCourseDirectoryApi();
    }

    [Fact]
    public async Task CourseDirectoryImportFunction_ManualImport_Returns_Expected_Result()
    {
        var service = Substitute.For<ICourseDirectoryDataService>();
        service
            .ImportProvidersFromCourseDirectoryApi()
            .Returns((10, 4));
        service
            .ImportQualificationsFromCourseDirectoryApi()
            .Returns((12, 2));

        var request = _functionContext.BuildHttpRequestData();

        var functions = new CourseDirectoryImportFunctionsBuilder().BuildCourseDirectoryImportFunctions(service);
        var result = await functions.ManualImport(request, _functionContext);

        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await result.Body.ReadAsString();
        body.Should().Be(
            "Inserted or updated 10 and deleted 4 providers.\r\n" +
            "Inserted or updated 12 and deleted 2 qualifications.");
    }

    [Fact]
    public async Task CourseDirectoryImportFunction_ManualImport_Exception_Returns_Expected_Result()
    {
        var service = Substitute.For<ICourseDirectoryDataService>();
        service
            .ImportProvidersFromCourseDirectoryApi()
            .ThrowsForAnyArgs(new InvalidOperationException());

        var request = _functionContext.BuildHttpRequestData();

        var functions = new CourseDirectoryImportFunctionsBuilder().BuildCourseDirectoryImportFunctions(service);
        var result = await functions.ManualImport(request, _functionContext);

        result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task CourseDirectoryImportFunction_GetCourseDirectoryDetailJson_Returns_Expected_Result()
    {
        const string expectedJson = "{ \"detail\": \"result\" } ";

        var service = Substitute.For<ICourseDirectoryDataService>();
        service
            .GetTLevelDetailJsonFromCourseDirectoryApi()
            .Returns(expectedJson);

        var request = _functionContext.BuildHttpRequestData();

        var functions = new CourseDirectoryImportFunctionsBuilder().BuildCourseDirectoryImportFunctions(service);
        var result = await functions.GetCourseDirectoryDetailJson(request, _functionContext);

        result.Headers.GetValues("Content-Type").Should().NotBeNull();
        result.Headers.GetValues("Content-Type").First().Should().Be("application/json");

        var body = await result.Body.ReadAsString();
        body.Should().Be(expectedJson);
    }

    [Fact]
    public async Task CourseDirectoryImportFunction_GetCourseDirectoryQualificationJson_Returns_Expected_Result()
    {
        const string expectedJson = "{ \"qualification\": \"result\" } ";

        var service = Substitute.For<ICourseDirectoryDataService>();
        service
            .GetTLevelQualificationJsonFromCourseDirectoryApi()
            .Returns(expectedJson);

        var request = _functionContext.BuildHttpRequestData();

        var functions = new CourseDirectoryImportFunctionsBuilder().BuildCourseDirectoryImportFunctions(service);
        var result = await functions.GetCourseDirectoryQualificationJson(request, _functionContext);

        result.Headers.GetValues("Content-Type").Should().NotBeNull();
        result.Headers.GetValues("Content-Type").First().Should().Be("application/json");

        var body = await result.Body.ReadAsString();
        body.Should().Be(expectedJson);
    }

    [Fact]
    public async Task CourseDirectoryImportFunction_GetCourseDirectoryDetailJson_Exception_Returns_Expected_Result()
    {
        var service = Substitute.For<ICourseDirectoryDataService>();
        service
            .GetTLevelDetailJsonFromCourseDirectoryApi()
            .ThrowsForAnyArgs(new InvalidOperationException());

        var request = _functionContext.BuildHttpRequestData();

        var functions = new CourseDirectoryImportFunctionsBuilder().BuildCourseDirectoryImportFunctions(service);
        var result = await functions.GetCourseDirectoryDetailJson(request, _functionContext);

        result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task CourseDirectoryImportFunction_GetCourseDirectoryQualificationJson_Exception_Returns_Expected_Result()
    {
        var service = Substitute.For<ICourseDirectoryDataService>();
        service
            .GetTLevelQualificationJsonFromCourseDirectoryApi()
            .ThrowsForAnyArgs(new InvalidOperationException());

        var request = _functionContext.BuildHttpRequestData();

        var functions = new CourseDirectoryImportFunctionsBuilder().BuildCourseDirectoryImportFunctions(service);
        var result = await functions.GetCourseDirectoryQualificationJson(request, _functionContext);

        result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task CourseDirectoryImportFunction_GetProviders_Returns_Expected_Result()
    {
        var builder = new ProviderBuilder();
        var providers = builder.BuildList();
        var expectedResult = builder.BuildJson().PrettifyJsonString();

        var tableStorageService = Substitute.For<ITableStorageService>();
        tableStorageService.GetAllProviders().Returns(providers);

        var request = _functionContext.BuildHttpRequestData();

        var functions = new CourseDirectoryImportFunctionsBuilder().BuildCourseDirectoryImportFunctions(tableStorageService: tableStorageService);
        var result = await functions.GetProviders(request, _functionContext);

        result.Headers.GetValues("Content-Type").Should().NotBeNull();
        result.Headers.GetValues("Content-Type").First().Should().Be("application/json");

        var json = await result.Body.ReadAsString();

        json.PrettifyJsonString().Should().Be(expectedResult);
    }

    [Fact]
    public async Task CourseDirectoryImportFunction_GetProviders_Exception_Returns_Expected_Result()
    {
        var tableStorageService = Substitute.For<ITableStorageService>();
        tableStorageService
            .GetAllProviders()
            .ThrowsForAnyArgs(new InvalidOperationException());

        var request = _functionContext.BuildHttpRequestData();

        var functions = new CourseDirectoryImportFunctionsBuilder().BuildCourseDirectoryImportFunctions(tableStorageService: tableStorageService);
        var result = await functions.GetProviders(request, _functionContext);

        result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task CourseDirectoryImportFunction_GetQualifications_Returns_Expected_Result()
    {
        var builder = new QualificationBuilder();
        var qualifications = builder.BuildList();
        var expectedResult = builder.BuildJson().PrettifyJsonString();

        var tableStorageService = Substitute.For<ITableStorageService>();
        tableStorageService.GetAllQualifications().Returns(qualifications);

        var request = _functionContext.BuildHttpRequestData();

        var functions = new CourseDirectoryImportFunctionsBuilder().BuildCourseDirectoryImportFunctions(tableStorageService: tableStorageService);
        var result = await functions.GetQualifications(request, _functionContext);

        result.Headers.GetValues("Content-Type").Should().NotBeNull();
        result.Headers.GetValues("Content-Type").First().Should().Be("application/json");

        var json = await result.Body.ReadAsString();
        json.PrettifyJsonString().Should().Be(expectedResult);
    }

    [Fact]
    public async Task CourseDirectoryImportFunction_GetQualifications_Exception_Returns_Expected_Result()
    {
        var tableStorageService = Substitute.For<ITableStorageService>();
        tableStorageService
            .GetAllQualifications()
            .ThrowsForAnyArgs(new InvalidOperationException());

        var request = _functionContext.BuildHttpRequestData();

        var functions = new CourseDirectoryImportFunctionsBuilder().BuildCourseDirectoryImportFunctions(tableStorageService: tableStorageService);
        var result = await functions.GetQualifications(request, _functionContext);

        result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }
}
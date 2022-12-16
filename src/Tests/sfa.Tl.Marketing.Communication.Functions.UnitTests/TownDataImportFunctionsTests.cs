using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSubstitute.ExceptionExtensions;
using sfa.Tl.Marketing.Communication.Application.Extensions;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Functions.UnitTests.Builders;
using sfa.Tl.Marketing.Communication.Functions.UnitTests.Extensions;
using sfa.Tl.Marketing.Communication.Tests.Common.Extensions;

namespace sfa.Tl.Marketing.Communication.Functions.UnitTests;

public class TownDataImportFunctionsTests
{
    [Fact]
    public void TownDataImportFunctions_Constructor_Guards_Against_NullParameters()
    {
        typeof(TownDataImportFunctions)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task TownDataImportFunctions_Scheduled_Import_Calls_TownDataService_Import_Methods()
    {
        var service = Substitute.For<ITownDataService>();
        service
            .ImportTowns()
            .Returns(10);

        var functionContext = FunctionObjectsBuilder.BuildFunctionContext();
        var request = FunctionObjectsBuilder.BuildHttpRequestData(HttpMethod.Post);

        var functions = TownDataImportFunctionsBuilder.Build(service);
        await functions.ImportTowns(
            request,
            functionContext);

        await service.Received(1).ImportTowns();
    }

    [Fact]
    public async Task TownDataImportFunctions_Scheduled_Import_Logs_Expected_Messages()
    {
        var service = Substitute.For<ITownDataService>();
        service
            .ImportTowns()
            .Returns(10);

        var logger = Substitute.For<ILogger<object>>();

        var functionContext = FunctionObjectsBuilder.BuildFunctionContext(logger);
        var request = FunctionObjectsBuilder.BuildHttpRequestData(HttpMethod.Post);

        var functions = TownDataImportFunctionsBuilder.Build(service);
        await functions.ImportTowns(
            request,
            functionContext);

        logger.HasLoggedMessage("Town data import saved 10 towns.");
    }

    [Fact]
    public async Task TownDataImportFunctions_Scheduled_Import_Exception_Logs_Expected_Messages()
    {
        var service = Substitute.For<ITownDataService>();
        service
            .ImportTowns()
            .ThrowsForAnyArgs(new InvalidOperationException());

        var logger = Substitute.For<ILogger<object>>();

        var functionContext = FunctionObjectsBuilder.BuildFunctionContext(logger);
        var request = FunctionObjectsBuilder.BuildHttpRequestData(HttpMethod.Post);

        var functions = TownDataImportFunctionsBuilder.Build(service);
        await functions.ImportTowns(
            request,
            functionContext);

        logger.ReceivedCalls().Count().Should().BeGreaterThan(0);
        logger.HasLoggedMessageLike("Error importing town data. " +
                                    "Internal Error Message System.InvalidOperationException",
            LogLevel.Error);
    }

    [Fact]
    public async Task TownDataImportFunctions_GetTowns_Returns_Expected_Result()
    {
        var builder = new TownBuilder();
        var towns = builder.BuildList();
        var expectedResult = builder.BuildJson().PrettifyJsonString();

        var tableStorageService = Substitute.For<ITableStorageService>();
        tableStorageService.GetAllTowns().Returns(towns);

        var functionContext = FunctionObjectsBuilder.BuildFunctionContext();
        var request = FunctionObjectsBuilder.BuildHttpRequestData(HttpMethod.Get);

        var functions = TownDataImportFunctionsBuilder.Build(tableStorageService: tableStorageService);
        var result = await functions.GetTowns(request, functionContext);

        result.Headers.GetValues("Content-Type").Should().NotBeNull();
        result.Headers.GetValues("Content-Type").First().Should().Be("application/json");

        var json = await result.Body.ReadAsString();

        json.PrettifyJsonString().Should().Be(expectedResult);
    }

    [Fact]
    public async Task TownDataImportFunctions_UploadData_Fails_For_Json_File()
    {
        var blobStorageService = Substitute.For<IBlobStorageService>();
        var functions = TownDataImportFunctionsBuilder.Build();

        await using var stream = TownDataImportFunctionsBuilder.BuildJsonFormDataStream();

        var functionContext = FunctionObjectsBuilder.BuildFunctionContext();
        var request = FunctionObjectsBuilder
            .BuildHttpRequestData(HttpMethod.Post, stream);

        var response = await functions.UploadData(
            request,
            functionContext);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        await blobStorageService
            .DidNotReceive()
            .Upload(Arg.Any<MemoryStream>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>());
    }

    [Fact]
    public async Task TownDataImportFunctions_UploadData_Succeeds_For_Csv_File()
    {
        var service = Substitute.For<ITownDataService>();
        service
            .ImportTowns()
            .Returns(10);

        var functions = TownDataImportFunctionsBuilder.Build(service);

        await using var stream = TownDataImportFunctionsBuilder.BuildTownCsvFormDataStream();

        var functionContext = FunctionObjectsBuilder.BuildFunctionContext();
        var request = FunctionObjectsBuilder
            .BuildHttpRequestData(HttpMethod.Post, stream);

        var response = await functions.UploadData(
            request,
            functionContext);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);

        await service.Received(1).ImportTownsFromCsvStream(Arg.Any<Stream>());
    }

    [Fact]
    public async Task TownDataImportFunctions_UploadData_Logs_Expected_Messages()
    {
        var service = Substitute.For<ITownDataService>();
        service
            .ImportTownsFromCsvStream(Arg.Any<Stream>())
            .Returns(10);

        var logger = Substitute.For<ILogger<object>>();

        var functions = TownDataImportFunctionsBuilder.Build(service);

        await using var stream = TownDataImportFunctionsBuilder.BuildTownCsvFormDataStream();

        var functionContext = FunctionObjectsBuilder.BuildFunctionContext(logger);
        var request = FunctionObjectsBuilder
            .BuildHttpRequestData(HttpMethod.Post, stream);

        await functions.UploadData(
            request,
            functionContext);

        logger.ReceivedCalls().Count().Should().Be(1);
        logger.HasLoggedMessage("Town data upload saved 10 towns.");
    }

    [Fact]
    public async Task TownDataImportFunctions_UploadData_Exception_Logs_Expected_Messages()
    {
        var service = Substitute.For<ITownDataService>();
        service
            .ImportTownsFromCsvStream(Arg.Any<Stream>())
            .ThrowsForAnyArgs(new InvalidOperationException());

        var logger = Substitute.For<ILogger<object>>();

        var functions = TownDataImportFunctionsBuilder.Build(service);

        await using var stream = TownDataImportFunctionsBuilder.BuildTownCsvFormDataStream();

        var functionContext = FunctionObjectsBuilder.BuildFunctionContext(logger);
        var request = FunctionObjectsBuilder
            .BuildHttpRequestData(HttpMethod.Post, stream);

        await functions.UploadData(
            request,
            functionContext);

        logger.HasLoggedMessageLike("Error reading or processing uploaded data. " +
                                    "Internal Error Message System.InvalidOperationException",
            LogLevel.Error);
    }
}
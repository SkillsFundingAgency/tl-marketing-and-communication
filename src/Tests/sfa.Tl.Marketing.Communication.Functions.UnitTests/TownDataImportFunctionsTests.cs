using System;
using System.Linq;
using System.Net;
using System.Net.Http;
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
using sfa.Tl.Marketing.Communication.Tests.Common.Extensions;
using Xunit;

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
    public async Task TownDataImportFunctions_Scheduled_Import_Calls_CourseDirectoryDataService_Import_Methods()
    {
        var service = Substitute.For<ITownDataService>();
        service
            .ImportTowns()
            .Returns(10);

        var functionContext = FunctionObjectsBuilder.BuildFunctionContext();
        var request = FunctionObjectsBuilder.BuildHttpRequestData(HttpMethod.Post);

        var functions = TownDataImportFunctionsBuilder.Build(service);
        await functions.ManualImport(
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
        await functions.ManualImport(
            request,
            functionContext);

        logger.ReceivedCalls().Count().Should().Be(3);

        logger.HasLoggedMessage("Town data manual import function was called.");

        logger.LogInformation("Town data manual saved 10 towns.");

        logger.HasLoggedMessage("Town data manual import finished.");
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
        await functions.ManualImport(
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
}
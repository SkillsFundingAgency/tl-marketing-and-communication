using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.Functions.Worker;
using NSubstitute;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Functions.UnitTests.Builders;
using sfa.Tl.Marketing.Communication.Functions.UnitTests.Extensions;
using Xunit;

namespace sfa.Tl.Marketing.Communication.Functions.UnitTests;

public class FindProviderApiImportFunctionTests
{
    //[Fact]
    //public void Employer_Controller_Constructor_Guards_Against_NullParameters()
    //{
    //    typeof(FindProviderApiImportFunctions)
    //        .ShouldNotAcceptNullConstructorArguments();
    //}

    [Fact]
    public async Task FindProviderImportFunction_Scheduled_Import_Calls_FindProviderApiDataService_Import_Methods()
    {
        var service = Substitute.For<IFindProviderApiDataService>();
        service
            .ImportProvidersFromFindProviderApi()
            .Returns((10, 0));
        service
            .ImportQualificationsFromFindProviderApi()
            .Returns((12, 0));

        var functionContext = FunctionObjectsBuilder.BuildFunctionContext();
        var functions = BuildFindProviderImportFunctions(service);
        await functions.ImportFindProviderApiData(
            new TimerInfo(),
            functionContext);

        await service.Received(1).ImportProvidersFromFindProviderApi();
        await service.Received(1).ImportQualificationsFromFindProviderApi();
    }


    [Fact]
    public async Task FindProviderImportFunction_GetFindProviderApiProvidersJson_Calls_FindProviderApiDataService_Methods()
    {
        const string expectedJson = "[ { \"ukPrn\": 12345678, \"providerName\": \"Test Provider\" } ]";

        var service = Substitute.For<IFindProviderApiDataService>();
        service
            .GetProvidersJsonFromFindProviderApi()
            .Returns(expectedJson);

        var functionContext = FunctionObjectsBuilder.BuildFunctionContext();
        var functions = BuildFindProviderImportFunctions(service);
        var request = FunctionObjectsBuilder.BuildHttpRequestData(HttpMethod.Get);

        var result = await functions.GetFindProviderApiProvidersJson(request, functionContext);

        result.Headers.GetValues("Content-Type").Should().NotBeNull();
        result.Headers.GetValues("Content-Type").First().Should().Be("application/json");

        var body = await result.Body.ReadAsString();
        body.Should().Be(expectedJson);
    }

    [Fact]
    public async Task FindProviderImportFunction_GetFindProviderApiQualificationsJson_Calls_FindProviderApiDataService_Methods()
    {
        const string expectedJson = "[ { \"id\": 47, \"name\": \"Accounting\" } ]";

        var service = Substitute.For<IFindProviderApiDataService>();
        service
            .GetQualificationsJsonFromFindProviderApi()
            .Returns(expectedJson);

        var functionContext = FunctionObjectsBuilder.BuildFunctionContext();
        var functions = BuildFindProviderImportFunctions(service);
        var request = FunctionObjectsBuilder.BuildHttpRequestData(HttpMethod.Get);

        var result = await functions.GetFindProviderApiQualificationsJson(request, functionContext);

        result.Headers.GetValues("Content-Type").Should().NotBeNull();
        result.Headers.GetValues("Content-Type").First().Should().Be("application/json");

        var body = await result.Body.ReadAsString();
        body.Should().Be(expectedJson);
    }

    private static FindProviderApiImportFunctions BuildFindProviderImportFunctions(
        IFindProviderApiDataService findProviderApiDataService = null)
    {
        findProviderApiDataService ??= Substitute.For<IFindProviderApiDataService>();

        return new FindProviderApiImportFunctions(findProviderApiDataService);
    }
}
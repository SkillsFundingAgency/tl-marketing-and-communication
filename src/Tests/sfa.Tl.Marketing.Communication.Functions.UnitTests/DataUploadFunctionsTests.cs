using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Functions.UnitTests.Builders;
using sfa.Tl.Marketing.Communication.Tests.Common.Extensions;
using Xunit;

namespace sfa.Tl.Marketing.Communication.Functions.UnitTests;

public class DataUploadFunctionsTests
{
    [Fact]
    public void DataUploadFunctions_Constructor_Guards_Against_NullParameters()
    {
        typeof(DataUploadFunctions)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task DataUploadFunctions_UploadProviderData_Processes_Json_File()
    {
        var blobStorageService = Substitute.For<IBlobStorageService>();
        var functions = DataUploadFunctionsBuilder.Build(blobStorageService);
        
        await using var stream = DataUploadFunctionsBuilder.BuildJsonFormDataStream();
        
        var functionContext = FunctionObjectsBuilder.BuildFunctionContext();
        var request = FunctionObjectsBuilder
            .BuildHttpRequestData(HttpMethod.Post, stream);

        var response = await functions.UploadProviderData(
            request,
            functionContext);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);

        await blobStorageService
            .Received(1)
            .Upload(Arg.Any<MemoryStream>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>());
    }


    [Fact]
    public async Task DataUploadFunctions_UploadProviderData_Fails_For_Non_Json_File()
    {
        var blobStorageService = Substitute.For<IBlobStorageService>();
        var functions = DataUploadFunctionsBuilder.Build(blobStorageService);

        await using var stream = DataUploadFunctionsBuilder.BuildCsvFormDataStream();

        var functionContext = FunctionObjectsBuilder.BuildFunctionContext();
        var request = FunctionObjectsBuilder
            .BuildHttpRequestData(HttpMethod.Post, stream);

        var response = await functions.UploadProviderData(
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
}
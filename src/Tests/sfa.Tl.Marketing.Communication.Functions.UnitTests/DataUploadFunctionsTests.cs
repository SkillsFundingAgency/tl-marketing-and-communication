using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
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

    //[Fact]
    //public void DataUploadFunctions_Constructor_Guards_Against_NullParameters_2()
    //{
    //    DataUploadFunctions target = null;
    //    Action act = () => target = new DataUploadFunctions((BlobServiceClient)null);

    //    act.Should().Throw<ArgumentNullException>();

    //    act.Should().Throw<ArgumentNullException>()
    //        .WithMessage(
    //            "Value cannot be null. (Parameter 'blobServiceClient')");
    //    target.Should().BeNull();
    //}

    //[Fact]
    //public void DataUploadFunctions_Constructor_Guards_Against_NullParameters_3()
    //{
    //    DataUploadFunctions target = null;
    //    Action act = () => target = new DataUploadFunctions(Substitute.For<BlobServiceClient>());

    //    act.Should().NotThrow<ArgumentNullException>();

    //    target.Should().NotBeNull();
    //}

    [Fact]
    public async Task DataUploadFunctions_UploadProviderData_Processes_File()
    {
        var blobClient = Substitute.For<BlobClient>();
        var blobContainerClient = Substitute.For<BlobContainerClient>();
        var blobServiceClient = Substitute.For<BlobServiceClient>();

        blobContainerClient
            .GetBlobClient(Arg.Any<string>())
            .Returns(blobClient);

        blobServiceClient
            .GetBlobContainerClient(Arg.Any<string>())
            .Returns(blobContainerClient);


        var blobStorageService = Substitute.For<IBlobStorageService>();
        var functions = DataUploadFunctionsBuilder.Build(blobStorageService);

        var testData =
            @"------WebKitFormBoundaryphElSb1aBJGfLyAP
Content-Disposition: form-data; name=""fileName""

Testfile
------WebKitFormBoundaryphElSb1aBJGfLyAP
Content-Disposition: form-data; name=""file""; filename=""Testfile.json""
Content-Type: application/json

{
    ""testData"": ""value""
}
"
            //+ new string('\0', 8147)
            + @"
------WebKitFormBoundaryphElSb1aBJGfLyAP--
";
        var stream = new MemoryStream();
        await using var writer = new StreamWriter(
            stream,
            //leaveOpen: true, 
            encoding: Encoding.UTF8);
        //await writer.WriteAsync("{ \"test\": 123 }");
        await writer.WriteAsync(testData);
        await writer.FlushAsync();
        stream.Position = 0;

        //await using var stream = await testData.ToStream();
        
        var functionContext = FunctionObjectsBuilder.BuildFunctionContext();
        var request = FunctionObjectsBuilder
            .BuildHttpRequestData(HttpMethod.Post, stream);
        //formFileCollection.

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
        //await blobClient
        //    .Received(1)
        //    .UploadAsync(Arg.Any<MemoryStream>(),
        //        Arg.Any<BlobHttpHeaders>());

        //await service.Received(1).ImportQualificationsFromCourseDirectoryApi();
        //await service.Received(1).ImportProvidersFromCourseDirectoryApi();
    }

    public static string TrimAllLines(string input)
    {
        return
            string.Concat(
                input.Split('\n')
                    .Select(x => x.Trim())
                    .Aggregate((first, second) => first + '\n' + second)
                    .Where(x => x != '\r'));
    }
}
using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using sfa.Tl.Marketing.Communication.Application.Services;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Application.Services;

public class BlobStorageServiceTests
{
    private const string ContainerName = "test-container";
    private const string FileName = "test-file";
    private const string ContentType = "application/json";

    [Fact]
    public void BlobStorageService_Constructor_Guards_Against_Null_BlobServiceClient()
    {
        BlobStorageService target = null;
        Action act = () => target = new BlobStorageService(null, Substitute.For<ILogger<BlobStorageService>>());

        act.Should().Throw<ArgumentNullException>();

        act.Should().Throw<ArgumentNullException>()
            .WithMessage(
                "Value cannot be null. (Parameter 'blobServiceClient')");
        target.Should().BeNull();
    }

    [Fact]
    public void BlobStorageService_Constructor_Guards_Against_Null_Logger()
    {
        BlobStorageService target = null;
        Action act = () => target = new BlobStorageService(Substitute.For<BlobServiceClient>(), null);

        act.Should().Throw<ArgumentNullException>();

        act.Should().Throw<ArgumentNullException>()
            .WithMessage(
                "Value cannot be null. (Parameter 'logger')");
        target.Should().BeNull();
    }

    [Fact]
    public void BlobStorageService_Constructor_Guards_Succeeds_With_Non_Null_Parameters()
    {
        BlobStorageService target = null;
        Action act = () => target = new BlobStorageService(Substitute.For<BlobServiceClient>(), Substitute.For<ILogger<BlobStorageService>>());

        act.Should().NotThrow<ArgumentNullException>();

        target.Should().NotBeNull();
    }

    [Fact]
    public async Task BlobStorageService_Upload_Calls_Blob_Client()
    {
        var blobClient = Substitute.For<BlobClient>();
        var blobContainerClient = Substitute.For<BlobContainerClient>();
        blobContainerClient
            .GetBlobClient(Arg.Any<string>())
            .Returns(blobClient);

        var blobServiceClient = Substitute.For<BlobServiceClient>();
        blobServiceClient
            .GetBlobContainerClient(Arg.Any<string>())
            .Returns(blobContainerClient);
        
        var service = BuildBlobStorageService(blobServiceClient);

        var stream = new MemoryStream();
        await service.Upload(stream, ContainerName, FileName, ContentType);

        await blobClient
            .Received(1)
            .UploadAsync(stream,
                Arg.Is<BlobHttpHeaders>(x =>
                    x.ContentType == ContentType));

    }

    [Fact]
    public async Task BlobStorageService_Get_Calls_Blob_Client()
    {
        var bytes = new byte[] { 104, 101, 108, 108, 111 }; //"hello"
        var blobStream = new MemoryStream(bytes);
        blobStream.Seek(0, SeekOrigin.Begin);

        var blobClient = Substitute.For<BlobClient>();
        await blobClient.DownloadToAsync(
            Arg.Do<Stream>(s =>
                blobStream.CopyTo(s)));

        var blobContainerClient = Substitute.For<BlobContainerClient>();
        blobContainerClient
            .GetBlobClient(Arg.Any<string>())
            .Returns(blobClient);

        var blobServiceClient = Substitute.For<BlobServiceClient>();
        blobServiceClient
            .GetBlobContainerClient(Arg.Any<string>())
            .Returns(blobContainerClient);

        var service = BuildBlobStorageService(blobServiceClient);

        var stream = await service.Get(ContainerName, FileName);

        stream.Should().NotBeNull();
        stream.CanRead.Should().BeTrue();
        stream.Length.Should().Be(5);
        stream.Position.Should().Be(0);
        //stream.Seek(0, SeekOrigin.Begin);

        using var reader = new StreamReader(stream);    
        (await reader.ReadToEndAsync()).Should().Be("hello");

        await blobClient
            .Received(1)
            .DownloadToAsync(Arg.Any<Stream>());
    }

    private static BlobStorageService BuildBlobStorageService(
        BlobServiceClient blobServiceClient = null,
        ILogger<BlobStorageService> logger = null)
    {
        blobServiceClient ??= Substitute.For<BlobServiceClient>();
        logger ??= Substitute.For<ILogger<BlobStorageService>>();

        return new BlobStorageService(
            blobServiceClient,
            logger);
    }
}
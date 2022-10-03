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
    public async Task BlobStorageService_ClearProviders_Returns_Expected_Results()
    {
        const string containerName = "test-container";
        const string fileName = "test-file";
        const string contentType = "application/json";

        var blobClient = Substitute.For<BlobClient>();
        var blobContainerClient = Substitute.For<BlobContainerClient>();
        var blobServiceClient = Substitute.For<BlobServiceClient>();

        blobContainerClient
            .GetBlobClient(Arg.Any<string>())
            .Returns(blobClient);

        blobServiceClient
            .GetBlobContainerClient(Arg.Any<string>())
            .Returns(blobContainerClient);
        
        var service = BuildBlobStorageService(blobServiceClient);

        var stream = new MemoryStream();
        await service.Upload(stream, containerName, fileName, contentType);

        await blobClient
            .Received(1)
            .UploadAsync(Arg.Any<MemoryStream>(),
                Arg.Any<BlobHttpHeaders>());

        await blobClient
            .Received(1)
            .UploadAsync(stream,
                Arg.Any<BlobHttpHeaders>());

        await blobClient
            .Received(1)
            .UploadAsync(stream,
                Arg.Is<BlobHttpHeaders>(x =>
                    x.ContentType == contentType));

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
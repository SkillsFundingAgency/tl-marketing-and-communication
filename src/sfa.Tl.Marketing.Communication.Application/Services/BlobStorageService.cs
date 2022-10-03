using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using sfa.Tl.Marketing.Communication.Application.Interfaces;

namespace sfa.Tl.Marketing.Communication.Application.Services;

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;

    private readonly ILogger<BlobStorageService> _logger;

    public BlobStorageService(
        BlobServiceClient blobServiceClient,
        ILogger<BlobStorageService> logger)
    {
        _blobServiceClient = blobServiceClient ?? throw new ArgumentNullException(nameof(blobServiceClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Upload(Stream stream,
            string containerName,
            string fileName,
            string contentType)
    {
        try
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await blobContainerClient.CreateIfNotExistsAsync();
            var blobClient = blobContainerClient.GetBlobClient(fileName);

            await blobClient.UploadAsync(stream,
                httpHeaders: new BlobHttpHeaders
                {
                    ContentType = contentType
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Blob upload filed ");
        }
    }
}
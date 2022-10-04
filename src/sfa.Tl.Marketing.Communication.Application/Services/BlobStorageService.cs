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
            var blobClient = await GetBlobClient(containerName, fileName);

            await blobClient.UploadAsync(stream,
                httpHeaders: new BlobHttpHeaders
                {
                    ContentType = contentType
                });

            _logger.LogInformation("Blob uploaded file {fileName} to container {contentType}. File size {streamLength}",
                fileName, contentType, stream.Length);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Blob upload failed.");
            throw;
        }
    }

    public async Task<Stream> Get(
        string containerName,
        string fileName)
    {
        try
        {
            var blobClient = await GetBlobClient(containerName, fileName);

            var stream = new MemoryStream();
            var response = await blobClient.DownloadToAsync(stream);
            if (stream.CanRead)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

            return stream;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Blob download failed.");
            //throw;
            return null;
        }
    }

    public async Task<BlobClient> GetBlobClient(
        string containerName,
        string fileName)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await blobContainerClient.CreateIfNotExistsAsync();
        return blobContainerClient.GetBlobClient(fileName);
    }
}
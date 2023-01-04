using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using System;
using sfa.Tl.Marketing.Communication.Models.Configuration;

namespace sfa.Tl.Marketing.Communication.Extensions;

public static class DataProtectionExtensions
{
    private const string ContainerName = "dataprotection";
    private const string BlobName = "keys";

    public static IServiceCollection AddWebDataProtection(
        this IServiceCollection services,
        ConfigurationOptions configuration)
    {
        if (!string.IsNullOrEmpty(configuration.StorageSettings.BlobStorageConnectionString))
        {
            services.AddDataProtection()
                .PersistKeysToAzureBlobStorage(
                    GetDataProtectionBlobTokenUri(configuration));
        }

        return services;
    }

    private static Uri GetDataProtectionBlobTokenUri(ConfigurationOptions configuration)
    {
        var blobServiceClient = new BlobServiceClient(configuration.StorageSettings.BlobStorageConnectionString);
        var blobContainerClient = blobServiceClient
            .GetBlobContainerClient(ContainerName);
        blobContainerClient.CreateIfNotExists();

        var blobClient = blobContainerClient
            .GetBlobClient(BlobName);

        return blobClient
            .GenerateSasUri(
                BlobSasPermissions.Read |
                BlobSasPermissions.Write |
                BlobSasPermissions.Create,
                DateTime.UtcNow.AddYears(1));
    }
}
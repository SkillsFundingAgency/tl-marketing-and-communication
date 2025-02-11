using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using System;
using sfa.Tl.Marketing.Communication.Models.Configuration;

namespace sfa.Tl.Marketing.Communication.Extensions
{
    public static class DataProtectionExtensions
    {
        private const string ContainerName = "dataprotection";
        private const string BlobName = "keys";

        public static IServiceCollection AddWebDataProtection(
            this IServiceCollection services,
            ConfigurationOptions siteConfiguration)
        {
            if (!string.IsNullOrEmpty(siteConfiguration.StorageSettings.StorageAccountName)
                && siteConfiguration.Environment != "LOCAL")
            {
                services.AddDataProtection()
                    .PersistKeysToAzureBlobStorage(
                        GetDataProtectionBlobUri(siteConfiguration));
            }

            return services;
        }

        private static Uri GetDataProtectionBlobUri(ConfigurationOptions siteConfiguration)
        {
            var blobServiceClient = new BlobServiceClient(
                new Uri($"https://{siteConfiguration.StorageSettings.StorageAccountName}.blob.core.windows.net"),
                new DefaultAzureCredential());
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
            try
            {
                blobContainerClient.CreateIfNotExists();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error ensuring container exists.", ex);
            }
            var blobClient = blobContainerClient.GetBlobClient(BlobName);
            return blobClient.Uri;
        }
    }
}
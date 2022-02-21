using System;
using System.Text.Json;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using sfa.Tl.Marketing.Communication.Application.Caching;
using sfa.Tl.Marketing.Communication.Models.Configuration;

namespace sfa.Tl.Marketing.Communication.Application.Extensions;

public static class ConfigurationExtensions
{
    public static ConfigurationOptions LoadConfigurationOptions(this IConfiguration configuration)
    {
        return configuration.LoadConfigurationOptions(
            configuration[ConfigurationKeys.EnvironmentNameConfigKey],
            configuration[ConfigurationKeys.ConfigurationStorageConnectionStringConfigKey],
            configuration[ConfigurationKeys.ServiceNameConfigKey],
            configuration[ConfigurationKeys.VersionConfigKey]);
    }

    public static ConfigurationOptions LoadConfigurationOptions(this IConfiguration configuration,
        string environment,
        string storageConnectionString,
        string serviceName,
        string version)
    {
        try
        {
            var requestOptions = new TableRequestOptions
            {
                MaximumExecutionTime = environment == "LOCAL"
                    ? TimeSpan.FromMilliseconds(500)
                    : TimeSpan.FromSeconds(30)
            };

            var conn = CloudStorageAccount.Parse(storageConnectionString);
            var tableClient = conn.CreateCloudTableClient();
            var table = tableClient.GetTableReference("Configuration");

            var operation = TableOperation.Retrieve(environment, $"{serviceName}_{version}");
            var result = table.ExecuteAsync(operation, requestOptions, null).GetAwaiter().GetResult();

            var dynResult = result.Result as DynamicTableEntity;
            var data = dynResult?.Properties["Data"].StringValue;

            if (data == null)
            {
                throw new NullReferenceException("Configuration data was null.");
            }

            return JsonSerializer.Deserialize<ConfigurationOptions>(data,
                new JsonSerializerOptions
                {
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    AllowTrailingCommas = true
                });
        }
        catch (StorageException e)
        {
            if (environment is "LOCAL" && e.InnerException is TimeoutException)
            {
                //Workaround to allow front-end developers to load config from app settings
                return null;
            }

            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "Configuration could not be loaded. Please check your configuration files or see the inner exception for details",
                ex);
        }
    }

    public static ConfigurationOptions LoadConfigurationOptionsFromAppSettings(this IConfiguration configuration)
    {
        return new ConfigurationOptions
        {
            CacheExpiryInSeconds = int.TryParse(configuration[ConfigurationKeys.CacheExpiryInSecondsConfigKey], out var cacheExpiryInSeconds)
                ? cacheExpiryInSeconds
                : CacheUtilities.DefaultCacheExpiryInSeconds,
            PostcodeCacheExpiryInSeconds = int.TryParse(configuration[ConfigurationKeys.PostcodeCacheExpiryInSecondsConfigKey], out var postcodeCacheExpiryInSeconds)
                ? postcodeCacheExpiryInSeconds
                : CacheUtilities.DefaultCacheExpiryInSeconds,
            EmployerSupportSiteUrl = configuration[ConfigurationKeys.EmployerSupportSiteUrlConfigKey],
            PostcodeRetrieverBaseUrl = configuration[ConfigurationKeys.PostcodeRetrieverBaseUrlConfigKey],
            StorageSettings = new StorageSettings
            {
                TableStorageConnectionString = configuration[ConfigurationKeys.TableStorageConnectionStringConfigKey]
            },
            MergeTempProviderData = bool.TryParse(configuration[ConfigurationKeys.MergeTempProviderDataConfigKey],
                                        out var mergeTempProviderData)
                                    && mergeTempProviderData,
        };
    }
}

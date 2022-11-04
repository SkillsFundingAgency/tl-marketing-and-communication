using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Data.Tables;
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
            var tableClient = new TableClient(storageConnectionString, "Configuration",
                environment == "LOCAL"
                    ? new TableClientOptions //Options to allow development running without azure emulator
                    {
                        Retry =
                        {
                            NetworkTimeout = TimeSpan.FromMilliseconds(500),
                            MaxRetries = 1
                        }
                    }
                    : default);

            var tableEntity = tableClient
                .Query<TableEntity>(
                    filter: $"PartitionKey eq '{environment}' and RowKey eq '{serviceName}_{version}'");

            var data = tableEntity.FirstOrDefault()?["Data"]?.ToString();

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
        catch (AggregateException aex)
        {
            if (environment is "LOCAL" && aex.InnerException is TaskCanceledException)
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
            Environment = configuration[ConfigurationKeys.EnvironmentNameConfigKey],
            CacheExpiryInSeconds = int.TryParse(configuration[ConfigurationKeys.CacheExpiryInSecondsConfigKey],
                out var cacheExpiryInSeconds)
                ? cacheExpiryInSeconds
                : CacheUtilities.DefaultCacheExpiryInSeconds,
            PostcodeCacheExpiryInSeconds =
                int.TryParse(configuration[ConfigurationKeys.PostcodeCacheExpiryInSecondsConfigKey],
                    out var postcodeCacheExpiryInSeconds)
                    ? postcodeCacheExpiryInSeconds
                    : CacheUtilities.DefaultCacheExpiryInSeconds,
            MergeTempProviderData = bool.TryParse(
                                        configuration[ConfigurationKeys.MergeTempProviderDataConfigKey],
                                        out var mergeTempProviderData)
                                    && mergeTempProviderData,
            PostcodeRetrieverBaseUrl = configuration[ConfigurationKeys.PostcodeRetrieverBaseUrlConfigKey],
            EmployerSiteSettings = new EmployerSiteSettings
            {
                SiteUrl = configuration[ConfigurationKeys.EmployerSupportSiteUriConfigKey],
                AboutArticle = configuration[ConfigurationKeys.AboutArticleConfigKey],
                IndustryPlacementsBenefitsArticle =
                    configuration[ConfigurationKeys.IndustryPlacementsBenefitsArticleConfigKey],
                SkillsArticle = configuration[ConfigurationKeys.SkillsArticleConfigKey],
                TimelineArticle = configuration[ConfigurationKeys.TimelineArticleConfigKey]
            },
            StorageSettings = new StorageSettings
            {
                TableStorageConnectionString =
                    configuration[ConfigurationKeys.TableStorageConnectionStringConfigKey]
            }
        };
    }
}
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Azure.Data.Tables;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using sfa.Tl.Marketing.Communication.Application.Extensions;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Application.Repositories;
using sfa.Tl.Marketing.Communication.Application.Services;
using sfa.Tl.Marketing.Communication.Functions.Extensions;
using sfa.Tl.Marketing.Communication.Models.Configuration;

var hostBuilder = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration(c =>
        {
            c.AddCommandLine(args)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: false)
                .AddJsonFile("local.settings.development.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables();
        }
    )
    .ConfigureServices((hostContext, services) =>
    {
        var config = hostContext.Configuration;
        var (apiConfiguration, storageConfiguration, environment) =
        (
            new CourseDirectoryApiSettings
            {
                ApiKey = config.GetConfigurationValue(ConfigurationKeys.CourseDirectoryApiKeyConfigKey),
                ApiBaseUri = config.GetConfigurationValue(ConfigurationKeys.CourseDirectoryApiBaseUriConfigKey)
            },
            new StorageSettings
            {
                TableStorageConnectionString = config.GetConfigurationValue(ConfigurationKeys.TableStorageConnectionStringConfigKey)
            },
            config.GetConfigurationValue(ConfigurationKeys.EnvironmentNameConfigKey)
        );

        var siteConfiguration = new ConfigurationOptions
        {
            Environment = environment,
            StorageConfiguration = storageConfiguration
        };

        services.AddSingleton(siteConfiguration);

        services.AddHttpClient<ICourseDirectoryDataService, CourseDirectoryDataService>(
                nameof(CourseDirectoryDataService),
                client =>
                {
                    client.BaseAddress = new Uri(apiConfiguration.ApiBaseUri);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiConfiguration.ApiKey);
                    client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                    client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                }
            )
            .ConfigurePrimaryHttpMessageHandler(_ =>
            {
                var handler = new HttpClientHandler();

                if (handler.SupportsAutomaticDecompression)
                {
                    handler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                }
                return handler;
            })
            .AddRetryPolicyHandler<CourseDirectoryDataService>();

        var tableServiceClient = new TableServiceClient(
            storageConfiguration.TableStorageConnectionString);

        services
            .AddSingleton(tableServiceClient)
            .AddTransient(typeof(ICloudTableRepository<>), typeof(GenericCloudTableRepository<>))
            .AddTransient<ICourseDirectoryDataService, CourseDirectoryDataService>()
            .AddTransient<ITableStorageService, TableStorageService>();
    });

var host = hostBuilder.Build();

await host.RunAsync();

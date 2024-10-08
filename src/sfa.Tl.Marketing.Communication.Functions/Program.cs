﻿using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using sfa.Tl.Marketing.Communication.Application.Extensions;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Application.Repositories;
using sfa.Tl.Marketing.Communication.Application.Services;
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
        var environment = Environment.GetEnvironmentVariable(ConfigurationKeys.EnvironmentNameConfigKey);

        var config = hostContext.Configuration.LoadConfigurationOptions(
                environment,
                Environment.GetEnvironmentVariable(ConfigurationKeys.ConfigurationStorageConnectionStringConfigKey),
                Environment.GetEnvironmentVariable(ConfigurationKeys.ServiceNameConfigKey),
                //NOTE: workaround issues with "Version" in local "Values" with .NET 6
                Environment.GetEnvironmentVariable(ConfigurationKeys.VersionConfigKey)
                ?? Environment.GetEnvironmentVariable(ConfigurationKeys.ServiceVersionConfigKey));

        var siteConfiguration = new ConfigurationOptions
        {
            Environment = environment
        };
        services.AddSingleton(siteConfiguration);

        services.AddHttpClient<ICourseDirectoryDataService, CourseDirectoryDataService>(
                nameof(CourseDirectoryDataService),
                client =>
                {
                    client.BaseAddress = new Uri(config.CourseDirectoryApiSettings.BaseUri);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", config.CourseDirectoryApiSettings.ApiKey);
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

        services
            .AddHttpClient<ITownDataService, TownDataService>(
                (_, client) =>
                {
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                }
            )
            .AddRetryPolicyHandler<TownDataService>();

        var tableServiceClient = new TableServiceClient(
            config.StorageSettings.TableStorageConnectionString);

        var blobServiceClient = new BlobServiceClient(
            config.StorageSettings.BlobStorageConnectionString);

        services.AddMemoryCache();

        services
            .AddSingleton(tableServiceClient)
            .AddSingleton(blobServiceClient)
            .AddTransient(typeof(ICloudTableRepository<>), typeof(GenericCloudTableRepository<>))
            .AddTransient<ICourseDirectoryDataService, CourseDirectoryDataService>()
            .AddTransient<ITownDataService, TownDataService>()
            .AddTransient<ITableStorageService, TableStorageService>();

        services
            .AddHealthChecks()
            .AddAzureBlobStorage()
            .AddAzureTable();
    });


var host = hostBuilder.Build();

await host.RunAsync();

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using sfa.Tl.Marketing.Communication.Application.Extensions;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Application.Repositories;
using sfa.Tl.Marketing.Communication.Application.Services;
using sfa.Tl.Marketing.Communication.Functions.Extensions;
using sfa.Tl.Marketing.Communication.Models.Configuration;

var host = new HostBuilder()
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
        var config = hostContext.Configuration.LoadConfigurationOptions(
                Environment.GetEnvironmentVariable(ConfigurationKeys.EnvironmentNameConfigKey),
                Environment.GetEnvironmentVariable(ConfigurationKeys.ConfigurationStorageConnectionStringConfigKey),
                Environment.GetEnvironmentVariable(ConfigurationKeys.ServiceNameConfigKey),
                //NOTE: workaround issues with "Version" in local "Values" with .NET 6
                Environment.GetEnvironmentVariable(ConfigurationKeys.VersionConfigKey)
                ?? Environment.GetEnvironmentVariable(ConfigurationKeys.ServiceVersionConfigKey));
        
        var siteConfiguration = new ConfigurationOptions
        {
            Environment = environment,
            StorageConfiguration = storageConfig
        };
        services.AddSingleton(siteConfiguration);
        RegisterHttpClients(services, config.CourseDirectoryApiSettings);
        RegisterServices(services, config.StorageSettings);
    })
    .Build();

await host.RunAsync();

static void RegisterHttpClients(IServiceCollection services, CourseDirectoryApiSettings apiConfiguration)
{
    services.AddHttpClient<ICourseDirectoryDataService, CourseDirectoryDataService>(
            nameof(CourseDirectoryDataService),
            client =>
            {
                client.BaseAddress = new Uri(apiConfiguration.BaseUri);
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
}

static void RegisterServices(IServiceCollection services, StorageSettings storageConfiguration)
{
    var cloudStorageAccount =
        CloudStorageAccount.Parse(storageConfiguration.TableStorageConnectionString);
    services.AddSingleton(cloudStorageAccount);
    var cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
    services.AddSingleton(cloudTableClient);

    services.AddTransient(typeof(ICloudTableRepository<>), typeof(GenericCloudTableRepository<>));
    services.AddTransient<ICourseDirectoryDataService, CourseDirectoryDataService>();
    services.AddTransient<ITableStorageService, TableStorageService>();
}

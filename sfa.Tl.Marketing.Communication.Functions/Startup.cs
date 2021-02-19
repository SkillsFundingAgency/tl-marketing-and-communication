using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Application.Repositories;
using sfa.Tl.Marketing.Communication.Application.Services;
using sfa.Tl.Marketing.Communication.Models.Configuration;
using sfa.Tl.Marketing.Communication.Models.Entities;

[assembly: FunctionsStartup(typeof(sfa.Tl.Marketing.Communication.Functions.Startup))]
namespace sfa.Tl.Marketing.Communication.Functions
{
    public class Startup : FunctionsStartup
    {
        protected CourseDirectoryApiSettings ApiConfiguration;
        protected StorageSettings StorageConfiguration;

        public override void Configure(IFunctionsHostBuilder builder)
        {
            LoadConfiguration();

            RegisterHttpClients(builder.Services);
            RegisterServices(builder.Services);
        }

        private void LoadConfiguration()
        {
            //https://stackoverflow.com/questions/59959258/how-to-add-an-appsettings-json-file-to-my-azure-function-3-0-configuration
            var config = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: false)
                .AddJsonFile("local.settings.development.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            ApiConfiguration = new CourseDirectoryApiSettings
            {
                ApiKey = GetConfigurationValue(config, ConfigurationKeys.CourseDirectoryApiKeyConfigKey),
                ApiBaseUri = GetConfigurationValue(config, ConfigurationKeys.CourseDirectoryApiBaseUriConfigKey)
            };

            StorageConfiguration = new StorageSettings
            {
                TableStorageConnectionString = GetConfigurationValue(config, ConfigurationKeys.TableStorageConnectionStringConfigKey)
            };
        }

        private static string GetConfigurationValue(IConfiguration config, string key)
        {
            var value = Environment.GetEnvironmentVariable(key);
            if (string.IsNullOrEmpty(value))
                value = config.GetValue<string>(key);
            if (string.IsNullOrEmpty(value))
                value = config.GetValue<string>($"Values:{key}");

            return value;
        }

        private void RegisterHttpClients(IServiceCollection services)
        {
            services.AddHttpClient<ICourseDirectoryDataService, CourseDirectoryDataService>(
                    CourseDirectoryDataService.CourseDirectoryHttpClientName,
                    client =>
                    {
                        client.BaseAddress = new Uri(ApiConfiguration.ApiBaseUri);
                        client.DefaultRequestHeaders.Add("Accept", "application/json");
                        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ApiConfiguration.ApiKey);
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
                });
        }

        private void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton(ApiConfiguration);
            services.AddSingleton(StorageConfiguration);

            var cloudStorageAccount =
                CloudStorageAccount.Parse(StorageConfiguration.TableStorageConnectionString);
            services.AddSingleton(cloudStorageAccount);
            var cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            services.AddSingleton(cloudTableClient);

            services.AddTransient(typeof(ICloudTableRepository<ProviderEntity>),
                typeof(GenericCloudTableRepository<ProviderEntity>));
            services.AddTransient(typeof(ICloudTableRepository<QualificationEntity>),
                typeof(GenericCloudTableRepository<QualificationEntity>));

            services.AddTransient<ICourseDirectoryDataService, CourseDirectoryDataService>();
            services.AddTransient<ITableStorageService, TableStorageService>();
        }
    }
}

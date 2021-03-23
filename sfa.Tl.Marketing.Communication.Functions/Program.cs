using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Application.Repositories;
using sfa.Tl.Marketing.Communication.Application.Services;
using sfa.Tl.Marketing.Communication.Functions.Extensions;
using sfa.Tl.Marketing.Communication.Models.Configuration;

namespace sfa.Tl.Marketing.Communication.Functions
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureAppConfiguration(c =>
                    {
                        c.AddCommandLine(args);
                        c.AddJsonFile("local.settings.json", optional: true, reloadOnChange: false);
                        c.AddJsonFile("local.settings.development.json", optional: true, reloadOnChange: false);
                        c.AddEnvironmentVariables();
                    }
                )
                .ConfigureServices((hostContext, services) =>
                {
                    var config = hostContext.Configuration;
                    var (apiConfig, storageConfig) =
                    (
                        new CourseDirectoryApiSettings
                        {
                            ApiKey = config.GetConfigurationValue(ConfigurationKeys.CourseDirectoryApiKeyConfigKey),
                            ApiBaseUri = config.GetConfigurationValue(ConfigurationKeys.CourseDirectoryApiBaseUriConfigKey)
                        },
                        new StorageSettings
                        {
                            TableStorageConnectionString = config.GetConfigurationValue(ConfigurationKeys.TableStorageConnectionStringConfigKey)
                        }
                    );

                    RegisterHttpClients(services, apiConfig);
                    RegisterServices(services, storageConfig);
                })
                .Build();

            await host.RunAsync();
        }

        private static void RegisterHttpClients(IServiceCollection services, CourseDirectoryApiSettings apiConfiguration)
        {
            services.AddHttpClient<ICourseDirectoryDataService, CourseDirectoryDataService>(
                    CourseDirectoryDataService.CourseDirectoryHttpClientName,
                    client =>
                    {
                        client.BaseAddress = new Uri(apiConfiguration.ApiBaseUri);
                        client.DefaultRequestHeaders.Add("Accept", "application/json");
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
                });
        }

        private static void RegisterServices(IServiceCollection services, StorageSettings storageConfiguration)
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
    }
}

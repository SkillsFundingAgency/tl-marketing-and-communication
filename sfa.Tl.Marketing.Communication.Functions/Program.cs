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
                        Console.WriteLine("Loading configuration ...");
                        c.AddCommandLine(args);
                        c.AddJsonFile("local.settings.json", optional: true, reloadOnChange: false);
                        c.AddJsonFile("local.settings.development.json", optional: true, reloadOnChange: false);
                        c.AddEnvironmentVariables();
                        Console.WriteLine("... Loaded configuration");
                    }
                )
                //.ConfigureServices(s =>
                .ConfigureServices((hostContext, services) =>
                {
                    Console.WriteLine("Configuring services ...");

                    var config = hostContext.Configuration;

                    Console.WriteLine("... Loading configuration ...");

                    LoadConfiguration(config);

                    Console.WriteLine("... Registering http clients ...");
                    RegisterHttpClients(services);

                    Console.WriteLine("... Registering services ...");
                    RegisterServices(services);

                    Console.WriteLine("... Configured services");
                })
                .Build();

            await host.RunAsync();
        }

        private static void LoadConfiguration(IConfiguration config)
        {
            ApiConfiguration = new CourseDirectoryApiSettings
            {
                ApiKey = config.GetConfigurationValue(ConfigurationKeys.CourseDirectoryApiKeyConfigKey),
                ApiBaseUri = config.GetConfigurationValue(ConfigurationKeys.CourseDirectoryApiBaseUriConfigKey)
            };

            Console.WriteLine($"   --> ApiConfiguration.ApiBaseUri {ApiConfiguration.ApiBaseUri}");
            StorageConfiguration = new StorageSettings
            {
                TableStorageConnectionString = config.GetConfigurationValue(ConfigurationKeys.TableStorageConnectionStringConfigKey)
            };
            Console.WriteLine($"   --> StorageConfiguration.TableStorageConnectionString {StorageConfiguration.TableStorageConnectionString}");
        }

        private static void RegisterHttpClients(IServiceCollection services)
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

        protected static CourseDirectoryApiSettings ApiConfiguration;
        protected static StorageSettings StorageConfiguration;

        private static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton(ApiConfiguration);
            services.AddSingleton(StorageConfiguration);

            var cloudStorageAccount =
                CloudStorageAccount.Parse(StorageConfiguration.TableStorageConnectionString);
            services.AddSingleton(cloudStorageAccount);
            var cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            services.AddSingleton(cloudTableClient);

            services.AddTransient(typeof(ICloudTableRepository<>), typeof(GenericCloudTableRepository<>));

            services.AddTransient<ICourseDirectoryDataService, CourseDirectoryDataService>();
            services.AddTransient<ITableStorageService, TableStorageService>();
        }
    }
}

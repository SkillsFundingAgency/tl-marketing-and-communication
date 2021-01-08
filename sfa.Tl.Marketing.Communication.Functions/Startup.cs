using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Application.Services;
using sfa.Tl.Marketing.Communication.Functions;
using sfa.Tl.Marketing.Communication.Models.Configuration;

[assembly: FunctionsStartup(typeof(Startup))]
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
                ApiKey = Environment.GetEnvironmentVariable(ConfigurationKeys.CourseDirectoryApiKeyConfigKey)
                         ?? config.GetValue<string>(ConfigurationKeys.CourseDirectoryApiKeyConfigKey),
                ApiBaseUri = Environment.GetEnvironmentVariable(ConfigurationKeys.CourseDirectoryApiBaseUriConfigKey)
                             ?? config.GetValue<string>(ConfigurationKeys.CourseDirectoryApiBaseUriConfigKey)
            };

            StorageConfiguration = new StorageSettings
            {
                TableStorageConnectionString = Environment.GetEnvironmentVariable(ConfigurationKeys.TableStorageConnectionStringConfigKey)
            };
        }

        private void RegisterHttpClients(IServiceCollection services)
        {
            /*
            services.AddHttpClient<ICourseDirectoryDataService, CourseDirectoryDataService>(
                    CourseDirectoryDataService.CourseDirectoryHttpClientName,
                    client =>
                    {
                        client.BaseAddress = new Uri(ApiConfiguration.ApiBaseUri);
                        client.DefaultRequestHeaders.Add("Accept", "application/json");
                        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ApiConfiguration.ApiKey);
                        // https://stackoverflow.com/questions/28754673/httpclient-conditionally-set-acceptencoding-compression-at-runtime
                        client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                        client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                    }
                )
                .ConfigurePrimaryHttpMessageHandler(messageHandler =>
                {
                    var handler = new HttpClientHandler();

                    if (handler.SupportsAutomaticDecompression)
                    {
                        handler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                    }
                    return handler;
                });
            */
        }

        private void RegisterServices(IServiceCollection services)
        {
            //    //https://stackoverflow.com/questions/57564396/how-do-i-mix-custom-parameter-binding-with-dependency-injection-in-azure-functio
            //    var webJobsBuilder = services.AddWebJobs(x => { });
            //    webJobsBuilder
            //        .AddAzureStorage()
            //        .AddAzureStorageCoreServices();

            //services.AddLogging(logging =>
            //{
            //    logging.AddConsole();
            //    logging.AddDebug();
            //    logging.AddApplicationInsights();
            //    logging.AddFilter((category, level) =>
            //        level >= (category == "Microsoft" ? LogLevel.Error : LogLevel.Information));
            //});
            
            services.AddSingleton(ApiConfiguration);
            services.AddSingleton(StorageConfiguration);
            
            //var cloudStorageAccount =
            //    CloudStorageAccount.Parse(StorageConfigurationOptions.TableStorageConnectionString);
            //services.AddSingleton(cloudStorageAccount);
            //var cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            //services.AddSingleton(cloudTableClient);

            //services.AddTransient(typeof(ICloudTableRepository<QualificationEntity>),
            //    typeof(GenericCloudTableRepository<QualificationEntity, int>));

            services.AddTransient<ICourseDirectoryDataService, CourseDirectoryDataService>();
            //services.AddTransient<IProviderStorageService, ProviderStorageService>();
        }
    }
}

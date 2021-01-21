using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using sfa.Tl.Marketing.Communication.Application.GeoLocations;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Application.Services;
using sfa.Tl.Marketing.Communication.Models.Configuration;
using sfa.Tl.Marketing.Communication.SearchPipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Notify.Client;
using Notify.Interfaces;
using sfa.Tl.Marketing.Communication.Application.Repositories;
using sfa.Tl.Marketing.Communication.Models.Dto;
using sfa.Tl.Marketing.Communication.Models.Entities;

namespace sfa.Tl.Marketing.Communication
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        protected ConfigurationOptions SiteConfiguration;
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public Startup(IConfiguration configuration, ILogger<Startup> logger, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            SiteConfiguration = new ConfigurationOptions
            {
                PostcodeRetrieverBaseUrl = Configuration["PostcodeRetrieverBaseUrl"],
                EmployerContactEmailTemplateId = Configuration["EmployerContactEmailTemplateId"],
                SupportEmailInboxAddress = Configuration["SupportEmailInboxAddress"],
                ProvidersDataFilePath = @$"{_webHostEnvironment.WebRootPath}\js\providers.json",
                QualificationsDataFilePath = @$"{_webHostEnvironment.WebRootPath}\js\qualifications.json",
                StorageConfiguration = new StorageSettings
                {
                    TableStorageConnectionString = Configuration[ConfigurationKeys.TableStorageConnectionStringConfigKey]
                }
            };

            services.AddApplicationInsightsTelemetry();

            services.AddSingleton(SiteConfiguration);

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAntiforgery(options =>
            {
                options.Cookie.Name = "tlevels-mc-x-csrf";
                options.FormFieldName = "_csrfToken";
                options.HeaderName = "X-XSRF-TOKEN";
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            RegisterHttpClients(services);
            RegisterServices(services);

            services.AddMemoryCache();

            var mvcBuilder = services.AddControllersWithViews()
                .AddMvcOptions(config =>
                {
                    config.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();
                });

            if (_webHostEnvironment.IsDevelopment())
            {
                mvcBuilder.AddRazorRuntimeCompilation();
            }
            else
            {
                services.AddHsts(options =>
                {
                    options.MaxAge = TimeSpan.FromDays(365);
                });
            }
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            IFileReader fileReader,
            ITableStorageService tableStorageService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseXContentTypeOptions();
            app.UseReferrerPolicy(opts => opts.NoReferrer());
            app.UseXXssProtection(opts => opts.EnabledWithBlockMode());
            app.UseXfo(xfo => xfo.Deny());

            app.UseCsp(options => options.ScriptSources(s => s
                        //.StrictDynamic()
                        .CustomSources("https:",
                            "https://www.google-analytics.com/analytics.js",
                            "https://www.googletagmanager.com/",
                            "https://tagmanager.google.com/",
                            "https://www.youtube.com/iframe_api")
                        .UnsafeInline()
                )
                .ObjectSources(s => s.None()));

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //TODO: Remove this after data is being pulled from the API
            PreloadProviderAndQualificationTableData(fileReader, tableStorageService).Wait();
        }

        protected virtual void RegisterHttpClients(IServiceCollection services)
        {
            services.AddHttpClient<ILocationApiClient, LocationApiClient>();
        }

        protected virtual void RegisterServices(IServiceCollection services)
        {
            services.AddTransient<IFileReader, FileReader>();
            services.AddSingleton<IProviderDataService, ProviderDataService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IDistanceCalculationService, DistanceCalculationService>();
            services.AddTransient<IJourneyService, JourneyService>();
            services.AddTransient<ILocationService, LocationService>();
            services.AddTransient<IProviderLocationService, ProviderLocationService>();
            services.AddTransient<IProviderSearchService, ProviderSearchService>();
            services.AddTransient<ISearchPipelineFactory, SearchPipelineFactory>();
            services.AddTransient<IProviderSearchEngine, ProviderSearchEngine>();

            var cloudStorageAccount =
                CloudStorageAccount.Parse(SiteConfiguration.StorageConfiguration.TableStorageConnectionString);
            services.AddSingleton(cloudStorageAccount);
            var cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            services.AddSingleton(cloudTableClient);

            services.AddTransient(typeof(ICloudTableRepository<ProviderEntity>),
                typeof(GenericCloudTableRepository<ProviderEntity, int>));
            services.AddTransient(typeof(ICloudTableRepository<QualificationEntity>),
                typeof(GenericCloudTableRepository<QualificationEntity, int>));

            services.AddTransient<ITableStorageService, TableStorageService>();

            var govNotifyApiKey = Configuration["GovNotifyApiKey"];
            services.AddTransient<IAsyncNotificationClient, NotificationClient>(
                provider =>
                    new NotificationClient(govNotifyApiKey));
        }

        private async Task PreloadProviderAndQualificationTableData(
            IFileReader fileReader,
            ITableStorageService tableStorageService)
        {
            try
            {
                _logger.LogInformation("Clearing table storage");
                
                var removedQualifications = await tableStorageService.ClearQualifications();
                _logger.LogInformation($"Removed {removedQualifications} qualifications from table storage");
                
                var savedQualifications = await tableStorageService
                    .SaveQualifications(GetQualificationsData(fileReader, SiteConfiguration.QualificationsDataFilePath));
                _logger.LogInformation($"Saved {savedQualifications} qualifications to table storage");

                var removedProviders = await tableStorageService.ClearProviders();
                _logger.LogInformation($"Removed {removedProviders} provider from table storage");

                var savedProviders = await tableStorageService
                    .SaveProviders(GetProvidersData(fileReader, SiteConfiguration.ProvidersDataFilePath));
                _logger.LogInformation($"Saved {savedProviders} providers to table storage");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save qualifications or providers to table storage");
            }
        }

        private static IList<Provider> GetProvidersData(IFileReader fileReader, string providersDataFilePath)
        {
            var json = fileReader.ReadAllText(providersDataFilePath);
            return JsonDocument.Parse(json)
                .RootElement
                .GetProperty("providers")
                .EnumerateArray()
                .Select(p =>
                    new Provider
                    {
                        Id = p.GetProperty("id").GetInt32(),
                        Name = p.GetProperty("name").GetString(),
                        Locations = p.GetProperty("locations")
                            .EnumerateArray()
                            .Select(l =>
                                new Location
                                {
                                    Postcode = l.GetProperty("postcode").GetString(),
                                    Name = l.GetProperty("name").GetString(),
                                    Town = l.GetProperty("town").GetString(),
                                    Latitude = l.GetProperty("latitude").GetDouble(),
                                    Longitude = l.GetProperty("longitude").GetDouble(),
                                    Website = l.GetProperty("website").GetString(),
                                    DeliveryYears = l.TryGetProperty("deliveryYears", out var deliveryYears)
                                        ? deliveryYears.EnumerateArray()
                                            .Select(d =>
                                                new DeliveryYearDto
                                                {
                                                    Year = d.GetProperty("year").GetInt16(),
                                                    Qualifications = d.GetProperty("qualifications")
                                                        .EnumerateArray()
                                                        .Select(q => q.GetInt32())
                                                        .ToList()
                                                })
                                            .ToList()
                                        : new List<DeliveryYearDto>()
                                }).ToList()
                    })
                .ToList();
        }

        private static IList<Qualification> GetQualificationsData(IFileReader fileReader, string qualificationsDataFilePath)
        {
            var json = fileReader.ReadAllText(qualificationsDataFilePath);
            return JsonDocument.Parse(json)
                .RootElement
                .GetProperty("qualifications")
                .EnumerateObject()
                .Select(q =>
                    new Qualification
                    {
                        Id = int.Parse(q.Name),
                        Name = q.Value.GetString()
                    })
                .OrderBy(q => q.Id)
                .ToList();
        }
    }
}

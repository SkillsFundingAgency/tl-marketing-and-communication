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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Notify.Client;
using Notify.Interfaces;
using sfa.Tl.Marketing.Communication.Application.Repositories;
using sfa.Tl.Marketing.Communication.Models.Entities;

namespace sfa.Tl.Marketing.Communication
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        protected ConfigurationOptions SiteConfiguration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger _logger;

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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
            services.AddTransient<ILocationService, LocationService>();
            services.AddTransient<IDistanceCalculationService, DistanceCalculationService>();
            services.AddTransient<IProviderLocationService, ProviderLocationService>();
            services.AddTransient<IProviderSearchService, ProviderSearchService>();
            services.AddTransient<ISearchPipelineFactory, SearchPipelineFactory>();
            services.AddTransient<IProviderSearchEngine, ProviderSearchEngine>();

            //TODO: Remove this - just want to confirm the value is being read
            _logger.LogInformation("Configuring table storage");
            var storageString = SiteConfiguration.StorageConfiguration?.TableStorageConnectionString ?? "null";
            storageString = storageString.Substring(0, Math.Min(storageString.Length - 1, 20));
            _logger.LogInformation($"Table storage string starts '{storageString}'");

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
    }
}

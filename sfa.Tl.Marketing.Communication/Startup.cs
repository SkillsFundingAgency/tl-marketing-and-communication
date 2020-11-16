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

namespace sfa.Tl.Marketing.Communication
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        protected ConfigurationOptions SiteConfiguration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            SiteConfiguration = new ConfigurationOptions
            {
                PostcodeRetrieverBaseUrl = Configuration["PostcodeRetrieverBaseUrl"],
                DataFilePath = @$"{_webHostEnvironment.WebRootPath}\js\providers.json"
            };

            services.AddSingleton(SiteConfiguration);

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            RegisterHttpClients(services);
            RegisterServices(services);

            services.AddMemoryCache();

            var mvcBuilder = services.AddControllersWithViews();
            
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
            services.AddTransient<ILocationService, LocationService>();
            services.AddTransient<IDistanceService, DistanceService>();
            services.AddTransient<IDistanceCalculationService, DistanceCalculationService>();
            services.AddTransient<IProviderLocationService, ProviderLocationService>();
            services.AddTransient<IProviderSearchService, ProviderSearchService>();
            services.AddTransient<ISearchPipelineFactory, SearchPipelineFactory>();
            services.AddTransient<IProviderSearchEngine, ProviderSearchEngine>();
        }
    }
}

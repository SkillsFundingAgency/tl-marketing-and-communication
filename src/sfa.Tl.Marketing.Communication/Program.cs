using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using sfa.Tl.Marketing.Communication.Application.Extensions;
using sfa.Tl.Marketing.Communication.Application.GeoLocations;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Application.Repositories;
using sfa.Tl.Marketing.Communication.Application.Services;
using sfa.Tl.Marketing.Communication.SearchPipeline;
using sfa.Tl.Marketing.Communication.SearchPipeline.Steps;

var builder = WebApplication.CreateBuilder(args);

builder.Logging
    .AddDebug()
    .AddConsole()
    .AddApplicationInsights(@"APPINSIGHTS_INSTRUMENTATIONKEY");

var programTypeName = MethodBase.GetCurrentMethod()?.DeclaringType?.FullName;
if (programTypeName != null)
{
    builder.Logging.AddFilter<Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider>(
        programTypeName, LogLevel.Trace);
}

var siteConfiguration = builder.Configuration.LoadConfigurationOptions()
                        ?? builder.Configuration.LoadConfigurationOptionsFromAppSettings();

builder.Services
    .AddApplicationInsightsTelemetry()
    .AddSingleton(siteConfiguration);

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
    options.CheckConsentNeeded = _ => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = "tlevels-mc-x-csrf";
    options.FormFieldName = "_csrfToken";
    options.HeaderName = "X-XSRF-TOKEN";
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services
    .AddHttpClient<ILocationApiClient, LocationApiClient>(
        client =>
        {
            client.BaseAddress = new Uri(siteConfiguration.PostcodeRetrieverBaseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        })
    .AddRetryPolicyHandler<LocationApiClient>();

builder.Services
    .AddTransient<IFileReader, FileReader>()
    .AddSingleton<IProviderDataService, ProviderDataService>()
    .AddTransient<IDateTimeService, DateTimeService>()
    .AddTransient<IDistanceCalculationService, DistanceCalculationService>()
    .AddTransient<IJourneyService, JourneyService>()
    .AddTransient<IProviderSearchService, ProviderSearchService>()
    .AddTransient<ISearchPipelineFactory, SearchPipelineFactory>()
    .AddTransient<ITownDataService, TownDataService>()
    .AddTransient<IProviderSearchEngine, ProviderSearchEngine>()
    .AddTransient<ISearchStep, GetQualificationsStep>()
    .AddTransient<ISearchStep, LoadSearchPageWithNoResultsStep>()
    .AddTransient<ISearchStep, ValidatePostcodeStep>()
    .AddTransient<ISearchStep, CalculateNumberOfItemsToShowStep>()
    .AddTransient<ISearchStep, PerformSearchStep>()
    .AddTransient<ISearchStep, MergeAvailableDeliveryYearsStep>();

var tableServiceClient = new TableServiceClient(
    siteConfiguration.StorageSettings.TableStorageConnectionString,
    siteConfiguration.Environment == "LOCAL" 
        ? new TableClientOptions //Options to allow development running without azure emulator
        {
            Retry =
            {
                NetworkTimeout = TimeSpan.FromMilliseconds(500),
                MaxRetries = 1
            }
        }
        : default);

var blobServiceClient = new BlobServiceClient(
    siteConfiguration.StorageSettings.BlobStorageConnectionString);

builder.Services
    .AddSingleton(tableServiceClient)
    .AddSingleton(blobServiceClient)
    .AddTransient(typeof(ICloudTableRepository<>), typeof(GenericCloudTableRepository<>))
    .AddTransient<ITableStorageService, TableStorageService>()
    .AddTransient<IBlobStorageService, BlobStorageService>();

builder.Services.AddMemoryCache();

var mvcBuilder = builder.Services.AddControllersWithViews()
    .AddMvcOptions(config =>
    {
        config.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();
    });

if (builder.Environment.IsDevelopment())
{
    mvcBuilder.AddRazorRuntimeCompilation();
}
else
{
    builder.Services.AddHsts(options =>
    {
        options.MaxAge = TimeSpan.FromDays(365);
    });
}

var assemblies = new[]
{
    typeof(Program).Assembly,
    typeof(Program).Assembly
        .GetReferencedAssemblies()
        .Where(a =>
            a.FullName.Contains("sfa.Tl.Marketing.Communication.Application"))
        .Select(Assembly.Load).FirstOrDefault()
};

builder.Services.AddAutoMapper(assemblies);

var app = builder.Build();

if (app.Environment.IsDevelopment())
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

app.Run();
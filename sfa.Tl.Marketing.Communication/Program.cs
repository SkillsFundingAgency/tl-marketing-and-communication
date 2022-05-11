using System;
using System.Net.Http.Headers;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using sfa.Tl.Marketing.Communication.Application.Caching;
using sfa.Tl.Marketing.Communication.Application.Extensions;
using sfa.Tl.Marketing.Communication.Application.GeoLocations;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Application.Repositories;
using sfa.Tl.Marketing.Communication.Application.Services;
using sfa.Tl.Marketing.Communication.Models.Configuration;
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

var siteConfiguration = new ConfigurationOptions
{
    CacheExpiryInSeconds = int.TryParse(builder.Configuration[ConfigurationKeys.CacheExpiryInSecondsConfigKey], out var cacheExpiryInSeconds)
            ? cacheExpiryInSeconds
            : CacheUtilities.DefaultCacheExpiryInSeconds,
    PostcodeCacheExpiryInSeconds = int.TryParse(builder.Configuration[ConfigurationKeys.PostcodeCacheExpiryInSecondsConfigKey], out var postcodeCacheExpiryInSeconds)
        ? postcodeCacheExpiryInSeconds
        : CacheUtilities.DefaultCacheExpiryInSeconds,
    MergeTempProviderData = bool.TryParse(builder.Configuration[ConfigurationKeys.MergeTempProviderDataConfigKey],
                                out var mergeTempProviderData)
                            && mergeTempProviderData,
    PostcodeRetrieverBaseUrl = builder.Configuration[ConfigurationKeys.PostcodeRetrieverBaseUrlConfigKey],
    EmployerSiteSettings = new EmployerSiteSettings
    {
        SiteUrl = builder.Configuration[ConfigurationKeys.EmployerSupportSiteUriConfigKey],
        AboutArticle = builder.Configuration[ConfigurationKeys.EmployerSupportSiteAboutArticleConfigKey],
        IndustryPlacementsBenefitsArticle = builder.Configuration[ConfigurationKeys.IndustryPlacementsBenefitsArticleConfigKey],
        SkillsArticle = builder.Configuration[ConfigurationKeys.SkillsArticleConfigKey],
        TimelineArticle = builder.Configuration[ConfigurationKeys.TimelineArticleConfigKey]
    },
    StorageConfiguration = new StorageSettings
    {
        TableStorageConnectionString = builder.Configuration[ConfigurationKeys.TableStorageConnectionStringConfigKey]
    }
};

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
    .AddTransient<IProviderSearchEngine, ProviderSearchEngine>()
    .AddTransient<ISearchStep, GetQualificationsStep>()
    .AddTransient<ISearchStep, LoadSearchPageWithNoResultsStep>()
    .AddTransient<ISearchStep, ValidatePostcodeStep>()
    .AddTransient<ISearchStep, CalculateNumberOfItemsToShowStep>()
    .AddTransient<ISearchStep, PerformSearchStep>()
    .AddTransient<ISearchStep, MergeAvailableDeliveryYearsStep>();

var cloudStorageAccount =
    CloudStorageAccount.Parse(siteConfiguration.StorageConfiguration.TableStorageConnectionString);
var cloudTableClient = cloudStorageAccount.CreateCloudTableClient();

builder.Services.AddSingleton(cloudStorageAccount)
    .AddSingleton(cloudTableClient)
    .AddTransient(typeof(ICloudTableRepository<>), typeof(GenericCloudTableRepository<>))
    .AddTransient<ITableStorageService, TableStorageService>();

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

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

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
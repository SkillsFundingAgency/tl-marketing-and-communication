using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models.Configuration;
using sfa.Tl.Marketing.Communication.Application.Extensions;

namespace sfa.Tl.Marketing.Communication.Application.Services;

public class FindProviderApiDataService : IFindProviderApiDataService
{
    public const string GetProvidersEndpoint = "providers/all";
    public const string GetQualificationsEndpoint = "qualifications";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ITableStorageService _tableStorageService;
    private readonly ILogger<FindProviderApiDataService> _logger;
    private readonly FindProviderApiSettings _apiSettings;

    public FindProviderApiDataService(
        IHttpClientFactory httpClientFactory,
        ITableStorageService tableStorageService,
        ConfigurationOptions siteConfiguration,
        ILogger<FindProviderApiDataService> logger)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _tableStorageService = tableStorageService ?? throw new ArgumentNullException(nameof(tableStorageService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        if (siteConfiguration is null) throw new ArgumentNullException(nameof(siteConfiguration));
        _apiSettings = siteConfiguration.FindProviderApiSettings;
    }

    public async Task<string> GetProvidersJsonFromFindProviderApi()
    {
        var httpClient = _httpClientFactory.CreateClient(nameof(FindProviderApiDataService));
        const string endpoint = GetProvidersEndpoint;

        httpClient.DefaultRequestHeaders.Authorization = 
            await new Uri(httpClient.BaseAddress, endpoint).ToString()
                .GetHmacHeader(
                    HttpMethod.Get.ToString(),
                    null,
                    _apiSettings.AppId,
                    _apiSettings.ApiKey);

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("{method} calling API {httpClientBaseAddress} endpoint {endpoint}",
                nameof(GetProvidersJsonFromFindProviderApi), httpClient.BaseAddress, endpoint);
        }

        var response = await httpClient.GetAsync(endpoint);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            _logger.LogError("API call to endpoint {endpoint} failed with {statusCode} - {reasonPhrase}",
                endpoint, response.StatusCode, response.ReasonPhrase);
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> GetQualificationsJsonFromFindProviderApi()
    {
        var httpClient = _httpClientFactory.CreateClient(nameof(FindProviderApiDataService));
        const string endpoint = GetQualificationsEndpoint;

        httpClient.DefaultRequestHeaders.Authorization =
            await new Uri(httpClient.BaseAddress, endpoint).ToString()
                .GetHmacHeader(
                    HttpMethod.Get.ToString(),
                null,
                    _apiSettings.AppId,
                    _apiSettings.ApiKey);

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("{method} calling API {httpClientBaseAddress} endpoint {endpoint}",
                nameof(GetQualificationsJsonFromFindProviderApi), httpClient.BaseAddress, endpoint);
        }

        var response = await httpClient.GetAsync(endpoint);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            _logger.LogError("API call to endpoint {endpoint} failed with {statusCode} - {reasonPhrase}",
                endpoint, response.StatusCode, response.ReasonPhrase);
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }


    public async Task<(int Saved, int Deleted)> ImportProvidersFromFindProviderApi()
    {
        var httpClient = _httpClientFactory.CreateClient(nameof(FindProviderApiDataService));
        const string endpoint = GetProvidersEndpoint;

        httpClient.DefaultRequestHeaders.Authorization =
            await new Uri(httpClient.BaseAddress, endpoint).ToString()
                .GetHmacHeader(
                    HttpMethod.Get.ToString(),
                    null,
                    _apiSettings.AppId,
                    _apiSettings.ApiKey);
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("{method} calling API {httpClientBaseAddress} endpoint {endpoint}",
                nameof(ImportProvidersFromFindProviderApi), httpClient.BaseAddress, endpoint);
        }

        var response = await httpClient.GetAsync(endpoint);

        var jsonDoc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());


        return (-1, -1);
    }

    public async Task<(int Saved, int Deleted)> ImportQualificationsFromFindProviderApi()
    {
        var httpClient = _httpClientFactory.CreateClient(nameof(FindProviderApiDataService));
        const string endpoint = GetQualificationsEndpoint;

        httpClient.DefaultRequestHeaders.Authorization =
            await new Uri(httpClient.BaseAddress, endpoint).ToString()
                .GetHmacHeader(
                    HttpMethod.Get.ToString(),
                    null,
                    _apiSettings.AppId,
                    _apiSettings.ApiKey);

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("{method} calling API {httpClientBaseAddress} endpoint {endpoint}",
                nameof(ImportQualificationsFromFindProviderApi), httpClient.BaseAddress, endpoint);
        }

        var response = await httpClient.GetAsync(endpoint);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            _logger.LogError("API call to endpoint {endpoint} failed with {statusCode} - {reasonPhrase}",
                endpoint, response.StatusCode, response.ReasonPhrase);
        }

        response.EnsureSuccessStatusCode();

        var jsonDoc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        //var qualifications = ProcessTLevelQualificationsDocument(jsonDoc);

        //return await UpdateQualificationsInTableStorage(qualifications);

        return (-1, -1);
    }
}
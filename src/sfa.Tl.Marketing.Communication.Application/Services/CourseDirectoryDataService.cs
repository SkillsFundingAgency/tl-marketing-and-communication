﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using sfa.Tl.Marketing.Communication.Application.Extensions;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models.Dto;
using sfa.Tl.Marketing.Communication.Models.Extensions;

namespace sfa.Tl.Marketing.Communication.Application.Services;

public class CourseDirectoryDataService : ICourseDirectoryDataService
{
    public const string CourseDetailEndpoint = "tlevels";
    public const string QualificationsEndpoint = "tleveldefinitions";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ITableStorageService _tableStorageService;
    private readonly ILogger<CourseDirectoryDataService> _logger;

    public CourseDirectoryDataService(
        IHttpClientFactory httpClientFactory,
        ITableStorageService tableStorageService,
        ILogger<CourseDirectoryDataService> logger)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _tableStorageService = tableStorageService ?? throw new ArgumentNullException(nameof(tableStorageService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<string> GetTLevelDetailJsonFromCourseDirectoryApi()
    {
        var httpClient = _httpClientFactory.CreateClient(nameof(CourseDirectoryDataService));
        const string endpoint = CourseDetailEndpoint;

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("{method} calling API {httpClientBaseAddress} endpoint {endpoint}",
                nameof(GetTLevelDetailJsonFromCourseDirectoryApi), httpClient.BaseAddress, endpoint);
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

    public async Task<string> GetTLevelQualificationJsonFromCourseDirectoryApi()
    {
        var httpClient = _httpClientFactory.CreateClient(nameof(CourseDirectoryDataService));
        const string endpoint = QualificationsEndpoint;

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("{method} calling API {httpClientBaseAddress} endpoint {endpoint}",
                nameof(GetTLevelQualificationJsonFromCourseDirectoryApi), httpClient.BaseAddress, endpoint);
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

    public async Task<(int Saved, int Deleted)> ImportProvidersFromCourseDirectoryApi()
    {
        var httpClient = _httpClientFactory.CreateClient(nameof(CourseDirectoryDataService));
        const string endpoint = CourseDetailEndpoint;

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("{method} calling API {httpClientBaseAddress} endpoint {endpoint}",
                nameof(ImportProvidersFromCourseDirectoryApi), httpClient.BaseAddress, endpoint);
        }

        var response = await httpClient.GetAsync(endpoint);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            _logger.LogError("API call to endpoint {endpoint} failed with {statusCode} - {reasonPhrase}",
                endpoint, response.StatusCode, response.ReasonPhrase);
        }

        response.EnsureSuccessStatusCode();

        var jsonDoc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var providers = ProcessTLevelDetailsDocument(jsonDoc);

        return await UpdateProvidersInTableStorage(providers);
    }

    public async Task<(int Saved, int Deleted)> ImportQualificationsFromCourseDirectoryApi()
    {
        var httpClient = _httpClientFactory.CreateClient(nameof(CourseDirectoryDataService));
        const string endpoint = QualificationsEndpoint;

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("{method} calling API {httpClientBaseAddress} endpoint {endpoint}",
                nameof(ImportQualificationsFromCourseDirectoryApi), httpClient.BaseAddress, endpoint);
        }

        var response = await httpClient.GetAsync(endpoint);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            _logger.LogError("API call to endpoint {endpoint} failed with {statusCode} - {reasonPhrase}",
                QualificationsEndpoint, response.StatusCode, response.ReasonPhrase);
        }

        response.EnsureSuccessStatusCode();

        var jsonDoc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var qualifications = ProcessTLevelQualificationsDocument(jsonDoc);

        //Workaround for rename of Education and Childcare
        // - this can be removed once Course Directory is updated
        var educationQualification = qualifications
            .SingleOrDefault(q => 
                q.Id == 38 && q.Name == "Education and Childcare");
        if (educationQualification != null)
        {
            var idx = qualifications.IndexOf(educationQualification);
            qualifications.RemoveAt(idx);
            qualifications.Insert(idx, 
                new Qualification
                {
                    Id = educationQualification.Id, 
                    Route = educationQualification.Route,
                    Name = "Education and Early Years"
                });
        }
        //End of workaround

        return await UpdateQualificationsInTableStorage(qualifications);
    }

    private IList<Provider> ProcessTLevelDetailsDocument(
        JsonDocument jsonDoc)
    {
        var providers = new List<Provider>();

        foreach (var courseElement in jsonDoc.RootElement
                     .GetProperty("tLevels")
                     .EnumerateArray()
                     .Where(courseElement => courseElement.SafeGetString("offeringType") == "TLevel"))
        {
            var tLevelId = courseElement.SafeGetString("tLevelId");

            if (!DateTime.TryParse(courseElement.SafeGetString("startDate"), out var startDate))
            {
                _logger.LogWarning("Could not read start date for course record with tLevelId {tLevelId}.", tLevelId);
                continue;
            }

            var qualification = courseElement.TryGetProperty("qualification", out var qualificationProperty)
                ? qualificationProperty.SafeGetInt32("frameworkCode")
                : 0;
            if (qualification == 0)
            {
                if (_logger.IsEnabled(LogLevel.Warning))
                {
                    _logger.LogWarning("Could not find qualification for course record with tLevelId {tLevelId}.",
                        tLevelId);
                }

                continue;
            }

            if (!courseElement.TryGetProperty("provider", out var providerProperty))
            {
                if (_logger.IsEnabled(LogLevel.Warning))
                {
                    _logger.LogWarning(
                        "Could not find provider property for course record with tLevelId {tLevelId}.", tLevelId);
                }
                continue;
            }

            if (!int.TryParse(providerProperty.SafeGetString("ukprn"), out var ukPrn))
            {
                if (_logger.IsEnabled(LogLevel.Warning))
                {
                    _logger.LogWarning(
                        "Could not find ukprn property for course record with tLevelId {tLevelId}.", tLevelId);
                }
                continue;
            }

            var providerName = providerProperty.SafeGetString("providerName");
            var providerWebsite = providerProperty.SafeGetString("website");

            var provider = providers.FirstOrDefault(p => p.UkPrn == ukPrn);

            if (provider == null)
            {
                provider = new Provider
                {
                    UkPrn = ukPrn,
                    Name = providerName,
                    Locations = new List<Location>()
                };
                providers.Add(provider);
            }

            if (!courseElement.TryGetProperty("locations", out var locationsProperty))
            {
                if (_logger.IsEnabled(LogLevel.Warning))
                {
                    _logger.LogWarning(
                        "Could not find locations property for course record with tLevelId {tLevelId}.", tLevelId);
                }
                continue;
            }

            foreach (var locationElement in locationsProperty.EnumerateArray())
            {
                var postcode = locationElement.SafeGetString("postcode");
                var locationWebsite = locationElement.SafeGetString("website");

                var location = provider.Locations.FirstOrDefault(l =>
                    l.Postcode == postcode);
                if (location == null)
                {
                    location = new Location
                    {
                        Postcode = postcode,
                        Name = locationElement.SafeGetString("venueName"),
                        Town = locationElement.SafeGetString("town"),
                        Latitude = locationElement.SafeGetDouble("latitude"),
                        Longitude = locationElement.SafeGetDouble("longitude"),
                        Website = !string.IsNullOrWhiteSpace(locationWebsite)
                            ? locationWebsite
                            : providerWebsite,
                        DeliveryYears = new List<DeliveryYearDto>()
                    };

                    provider.Locations.Add(location);
                }

                var deliveryYear = location
                    .DeliveryYears
                    .FirstOrDefault(dy => dy.Year == startDate.Year);
                if (deliveryYear == null)
                {
                    deliveryYear = new DeliveryYearDto
                    {
                        Year = (short)startDate.Year,
                        Qualifications = new List<int>
                        {
                            qualification
                        }
                    };
                    location.DeliveryYears.Add(deliveryYear);
                }
                else if (!deliveryYear.Qualifications
                             .Contains(qualification))
                {
                    deliveryYear.Qualifications.Add(qualification);
                }
            }
        }

        return providers;
    }

    private static IList<Qualification> ProcessTLevelQualificationsDocument(JsonDocument jsonDoc)
    {
        return jsonDoc.RootElement
            .GetProperty("tLevelDefinitions")
            .EnumerateArray()
            .Select(q =>
            {
                var (route, name) = ExtractQualificationRouteAndName(q.SafeGetString("name"));

                return new Qualification
                {
                    Id = q.SafeGetInt32("frameworkCode"),
                    Route = route,
                    Name = name
                };
            }).ToList();
    }

    private static (string Route, string Name) ExtractQualificationRouteAndName(string fullName)
    {
        string route = null;
        string name = null;
        if (!string.IsNullOrWhiteSpace(fullName))
        {
            var parts = fullName.Split('-');

            route = parts.Length > 1
                ? Regex.Replace(parts[0],
                        "^T Level", "", RegexOptions.IgnoreCase)
                    .ToTitleCase()
                : null;

            name = Regex.Replace(parts[^1],
                    "^T Level in", "", RegexOptions.IgnoreCase)
                .ToTitleCase();
        }

        return (route, name);
    }

    private async Task<(int Saved, int Deleted)> UpdateProvidersInTableStorage(IList<Provider> providers)
    {
        var existingProviders = await _tableStorageService.GetAllProviders();

        var providersToInsertOrUpdate = providers.Where(q =>
            ProviderIsNewOrHasChanges(existingProviders, q)
        ).ToList();

        var providersToDelete = existingProviders.Where(q =>
                providers.All(x => x.UkPrn != q.UkPrn)) //Not in new data, so add it to the delete list
            .ToList();

        var savedProviders = 0;
        if (providersToInsertOrUpdate.Any())
        {
            savedProviders = await _tableStorageService.SaveProviders(providersToInsertOrUpdate);
            _logger.LogInformation("Saved {savedProviders} providers to table storage", savedProviders);
        }

        var deletedProviders = 0;
        if (providersToDelete.Any())
        {
            deletedProviders = await _tableStorageService.RemoveProviders(providersToDelete);
            _logger.LogInformation("Deleted {deletedProviders} providers from table storage", savedProviders);

        }

        return (Saved: savedProviders, Deleted: deletedProviders);
    }

    private async Task<(int Saved, int Deleted)> UpdateQualificationsInTableStorage(IList<Qualification> qualifications)
    {
        var existingQualifications = await _tableStorageService.GetAllQualifications();

        var qualificationsToInsertOrUpdate = qualifications.Where(q =>
            QualificationIsNewOrHasChanges(existingQualifications, q)
        ).ToList();

        var qualificationsToDelete = existingQualifications.Where(q =>
                qualifications.All(x => x.Id != q.Id) //Not in new data, so add it to the delete list
        ).ToList();

        var savedQualifications = 0;
        if (qualificationsToInsertOrUpdate.Any())
        {
            savedQualifications = await _tableStorageService.SaveQualifications(qualificationsToInsertOrUpdate);
            _logger.LogInformation("Saved {savedQualifications} qualifications to table storage", savedQualifications);
        }

        var deletedQualifications = 0;
        if (qualificationsToDelete.Any())
        {
            deletedQualifications = await _tableStorageService.RemoveQualifications(qualificationsToDelete);
            _logger.LogInformation("Deleted {deletedQualifications} qualifications from table storage", deletedQualifications);
        }

        return (Saved: savedQualifications, Deleted: deletedQualifications);
    }

    private bool ProviderIsNewOrHasChanges(IEnumerable<Provider> existingProviders, Provider provider)
    {
        var existingProvider = existingProviders.FirstOrDefault(p => p.UkPrn == provider.UkPrn);
        if (existingProvider == null) return true; //This provider does not exist 

        var hasChanges = false;
        hasChanges |= existingProvider.Name != provider.Name;
        hasChanges |= existingProvider.Locations.Count != provider.Locations.Count;

        foreach (var location in existingProvider.Locations)
        {
            var matchingLocation = provider.Locations.FirstOrDefault(l => l.Postcode == location.Postcode);
            hasChanges |= matchingLocation == null;
            if (matchingLocation != null)
            {
                hasChanges |= LocationIsNewOrHasChanges(provider, location, matchingLocation);
            }
        }

        return hasChanges;
    }

    private bool LocationIsNewOrHasChanges(Provider provider, Location location, Location matchingLocation)
    {
        if (matchingLocation != null)
        {
            if (matchingLocation.Name != location.Name)
            {
                if (_logger.IsEnabled(LogLevel.Warning))
                {
                    _logger.LogWarning(
                        "Venue name for {provider.UkPrn} {provider.Name} {location.Postcode} " +
                        "changed from '{location.Name}' to '{matchingLocation.Name}'",
                        provider.UkPrn, provider.Name, location.Postcode,
                        location.Name, matchingLocation.Name);
                }

                //At this point we don't need to keep checking, because we have logged the venue name change
                return true;
            }

            if (matchingLocation.Town != location.Town ||
                matchingLocation.Website != location.Website ||
                Math.Abs(matchingLocation.Latitude - location.Latitude) > .000001 ||
                Math.Abs(matchingLocation.Longitude - location.Longitude) > .000001 ||
                matchingLocation.DeliveryYears.Count != location.DeliveryYears.Count)
                return true;

            foreach (var deliveryYear in matchingLocation.DeliveryYears)
            {
                var matchingDeliveryYear =
                    location.DeliveryYears.FirstOrDefault(dy => dy.Year == deliveryYear.Year);

                if (matchingDeliveryYear == null ||
                    matchingDeliveryYear.Qualifications.Count !=
                    deliveryYear.Qualifications.Count ||
                    matchingDeliveryYear.Qualifications.Any(qualification =>
                        !deliveryYear.Qualifications.Contains(qualification)))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool QualificationIsNewOrHasChanges(IEnumerable<Qualification> existingQualifications, Qualification qualification)
    {
        var existingQualification = existingQualifications.FirstOrDefault(q => q.Id == qualification.Id);
        return existingQualification == null
               || existingQualification.Route != qualification.Route
               || existingQualification.Name != qualification.Name;
    }
}
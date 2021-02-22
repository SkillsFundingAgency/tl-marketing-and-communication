using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace sfa.Tl.Marketing.Communication.Application.Services
{
    public class CourseDirectoryDataService : ICourseDirectoryDataService
    {
        public const string CourseDirectoryHttpClientName = "CourseDirectoryAutoCompressClient";
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
            var httpClient = _httpClientFactory.CreateClient(CourseDirectoryHttpClientName);

            _logger.LogInformation($"Call API {httpClient.BaseAddress} endpoint {CourseDetailEndpoint}");

            var response = await httpClient.GetAsync(CourseDetailEndpoint);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError($"API call failed with {response.StatusCode} - {response.ReasonPhrase}");
            }

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetTLevelQualificationJsonFromCourseDirectoryApi()
        {
            var httpClient = _httpClientFactory.CreateClient(CourseDirectoryHttpClientName);

            _logger.LogInformation($"Call API {httpClient.BaseAddress} endpoint {CourseDetailEndpoint}");

            var response = await httpClient.GetAsync(QualificationsEndpoint);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError($"API call failed with {response.StatusCode} - {response.ReasonPhrase}");
            }

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<(int Saved, int Deleted)> ImportProvidersFromCourseDirectoryApi(IList<VenueNameOverride> venueNames)
        {
            _logger.LogInformation("ImportFromCourseDirectoryApi called");

            var httpClient = _httpClientFactory.CreateClient(CourseDirectoryHttpClientName);

            _logger.LogInformation($"Call API {httpClient.BaseAddress} endpoint {CourseDetailEndpoint}");

            var response = await httpClient.GetAsync(CourseDetailEndpoint);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError($"API call failed with {response.StatusCode} - {response.ReasonPhrase}");
            }

            response.EnsureSuccessStatusCode();

            var jsonDoc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
            var providers = ProcessTLevelDetailsDocument(jsonDoc);

            return await UpdateProvidersInTableStorage(providers);
        }

        public async Task<(int Saved, int Deleted)> ImportQualificationsFromCourseDirectoryApi()
        {
            _logger.LogInformation("ImportFromCourseDirectoryApi called");

            var httpClient = _httpClientFactory.CreateClient(CourseDirectoryHttpClientName);

            _logger.LogInformation($"Call API {httpClient.BaseAddress} endpoint {CourseDetailEndpoint}");

            var response = await httpClient.GetAsync(QualificationsEndpoint);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError($"API call failed with {response.StatusCode} - {response.ReasonPhrase}");
            }

            response.EnsureSuccessStatusCode();

            var jsonDoc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
            var qualifications = ProcessTLevelQualificationsDocument(jsonDoc);

            return await UpdateQualificationsInTableStorage(qualifications);
        }
        
        private IList<Provider> ProcessTLevelDetailsDocument(JsonDocument jsonDoc)
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
                    _logger.LogWarning($"Could not read start date for course record with tLevelId {tLevelId}.");
                    continue;
                }

                var qualification = courseElement.TryGetProperty("qualification", out var qualificationProperty)
                    ? qualificationProperty.SafeGetInt32("frameworkCode")
                    : 0;
                if (qualification == 0)
                {
                    _logger.LogWarning("Could not find qualification for course record with tLevelId {tLevelId}.");
                    continue;
                }

                if (!courseElement.TryGetProperty("provider", out var providerProperty))
                {
                    _logger.LogWarning("Could not find provider property for course record with tLevelId {tLevelId}.");
                    continue;
                }

                var hasUkPrn = int.TryParse(providerProperty.SafeGetString("ukprn"), out var ukPrn);
                //TODO: Log and continue if no ukprn?

                var providerName = providerProperty.SafeGetString("providerName");
                var providerWebsite = providerProperty.SafeGetString("website");

                var provider = providers.SingleOrDefault(p => p.UkPrn == ukPrn);
                if (provider == null)
                {
                    provider = new Provider
                    {
                        //TODO: Should we use this as id? Or use UKPRN?
                        UkPrn = ukPrn,
                        Name = providerName,
                        Locations = new List<Location>()
                    };
                    providers.Add(provider);
                }

                if (!courseElement.TryGetProperty("locations", out var locationsProperty))
                {
                    _logger.LogWarning(
                        "Could not find locations property for course record with tLevelId {tLevelId}.");
                    continue;
                }

                foreach (var locationElement in locationsProperty.EnumerateArray())
                {
                    var postcode = locationElement.SafeGetString("postcode");
                    var locationWebsite = locationElement.SafeGetString("website");

                    var location = provider.Locations.SingleOrDefault(l =>
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

        private IList<Qualification> ProcessTLevelQualificationsDocument(JsonDocument jsonDoc)
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

        private (string Route, string Name) ExtractQualificationRouteAndName(string fullName)
        {
            string route = null;
            string name = null;
            if (!string.IsNullOrWhiteSpace(fullName))
            {
                var parts = fullName.Split('-');
                switch (parts.Length)
                {
                    case > 1:
                        route = Regex.Replace(parts[0],
                                $"^T Level", "")
                            .ToTitleCase();
                        name = parts[1].ToTitleCase();
                        break;
                    case 1:
                        name = parts[0].ToTitleCase();
                        break;
                }
            }

            Debug.WriteLine($"{route} => {name}");
            return (route, name);
        }

        private async Task<(int Saved, int Deleted)> UpdateProvidersInTableStorage(IList<Provider> providers)
        {
            var existingProviders = await _tableStorageService.GetAllProviders();

            var stopwatch = Stopwatch.StartNew();
            
            var providersToInsertOrUpdate = providers.Where(q =>
                ProviderIsNewOrHasChanges(existingProviders, q)
            ).ToList();

            stopwatch.Stop();
            Debug.WriteLine($"ProviderIsNewOrHasChanges took {stopwatch.ElapsedMilliseconds}ms {stopwatch.ElapsedTicks} ticks");

            var providersToDelete = existingProviders.Where(q =>
                    providers.All(x => x.UkPrn != q.UkPrn) //Not in new data, so add it to the delete list
            ).ToList();

            var savedProviders = 0;
            if (providersToInsertOrUpdate.Any())
            {
                savedProviders = await _tableStorageService.SaveProviders(providersToInsertOrUpdate);
                _logger.LogInformation($"Saved {savedProviders} providers to table storage");
            }

            var deletedProviders = 0;
            if (providersToDelete.Any())
            {
                deletedProviders = await _tableStorageService.RemoveProviders(providersToDelete);
                _logger.LogInformation($"Deleted {deletedProviders} providers from table storage");
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
                _logger.LogInformation($"Saved {savedQualifications} qualifications to table storage");
            }

            var deletedQualifications = 0;
            if (qualificationsToDelete.Any())
            {
                deletedQualifications = await _tableStorageService.RemoveQualifications(qualificationsToDelete);
                _logger.LogInformation($"Deleted {deletedQualifications} qualifications from table storage");
            }

            return (Saved: savedQualifications, Deleted: deletedQualifications);
        }

        private bool ProviderIsNewOrHasChanges(IEnumerable<Provider> existingProviders, Provider provider)
        {
            var existingProvider = existingProviders.SingleOrDefault(p => p.UkPrn == provider.UkPrn);
            if (existingProvider == null) return true; //This provider does not exist 

            var hasChanges = false;
            hasChanges |= existingProvider.Name != provider.Name;
            hasChanges |= existingProvider.Locations.Count != provider.Locations.Count;

            foreach (var location in existingProvider.Locations)
            {
                var matchingLocation = provider.Locations.SingleOrDefault(l => l.Postcode == location.Postcode);
                hasChanges |= matchingLocation == null;

                if (matchingLocation != null)
                {
                    hasChanges |= matchingLocation.Town != location.Town;
                    if (matchingLocation.Name != location.Name)
                    {
                        hasChanges = true;
                        _logger.LogWarning($"Venue name for {provider.UkPrn} {provider.Name} {location.Postcode} " +
                                           $"changed from '{location.Name}' to '{matchingLocation.Name}'");
                    }

                    hasChanges |= matchingLocation.Website != location.Website;

                    hasChanges |= Math.Abs(matchingLocation.Latitude - location.Latitude) > .000001;
                    hasChanges |= Math.Abs(matchingLocation.Longitude - location.Longitude) > .000001;

                    hasChanges |= matchingLocation.DeliveryYears.Count != location.DeliveryYears.Count;

                    foreach (var deliveryYear in matchingLocation.DeliveryYears)
                    {
                        var matchingDeliveryYear =
                            location.DeliveryYears.SingleOrDefault(dy => dy.Year == deliveryYear.Year);
                        hasChanges |= matchingDeliveryYear == null;

                        if (matchingDeliveryYear != null)
                        {
                            hasChanges |= matchingDeliveryYear.Qualifications.Count !=
                                          deliveryYear.Qualifications.Count;

                            hasChanges |= (matchingDeliveryYear.Qualifications.Any(qualification =>
                                !deliveryYear.Qualifications.Contains(qualification)));
                        }
                    }
                }
            }

            return hasChanges;
        }

        private bool QualificationIsNewOrHasChanges(IEnumerable<Qualification> existingQualifications, Qualification qualification)
        {
            var existingQualification = existingQualifications.SingleOrDefault(q => q.Id == qualification.Id);
            if (existingQualification == null) return true; //This qualification does not exist 

            var hasChanges = false;
            hasChanges |= existingQualification.Route != qualification.Route;
            hasChanges |= existingQualification.Name != qualification.Name;

            return hasChanges;
        }
    }
}
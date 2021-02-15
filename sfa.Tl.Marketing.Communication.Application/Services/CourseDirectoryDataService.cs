using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using sfa.Tl.Marketing.Communication.Application.Extensions;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.Application.Services
{
    public class CourseDirectoryDataService : ICourseDirectoryDataService
    {
        public const string CourseDirectoryHttpClientName = "CourseDirectoryAutoCompressClient";
        public const string CourseDetailEndpoint = "tleveldetail";
        public const string QualificationsEndpoint = "tlevelqualification";
        public const string QualificationTitlePrefix = "T Level Technical Qualification in ";

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
                response = CreateWorkaroundResponse();
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
                response = CreateWorkaroundResponse("CourseDirectoryTLevelQualificationsResponse");
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
                response = CreateWorkaroundResponse();
            }
            else
            {
                //TODO: Remove this - the new API shouldn't need to be re-wrapped in []
                //var content = await response.Content.ReadAsStringAsync();
                //content = $"[\n{content}\n]";
                //response.Content = new StringContent(content);
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

            var response = await httpClient.GetAsync(CourseDetailEndpoint);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError($"API call failed with {response.StatusCode} - {response.ReasonPhrase}");
                response = CreateWorkaroundResponse("CourseDirectoryTLevelQualificationsResponse");
            }

            response.EnsureSuccessStatusCode();

            var jsonDoc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
            var qualifications = ProcessTLevelQualificationsDocument(jsonDoc);

            return  await UpdateQualificationsInTableStorage(qualifications);
        }

        public async Task<IList<Provider>> GetProviders()
        {
            return (await _tableStorageService.GetAllProviders())
                .OrderBy(p => p.Id).ToList();
        }

        public async Task<IList<Qualification>> GetQualifications()
        {
            return (await _tableStorageService
                    .GetAllQualifications())
                .OrderBy(q => q.Id).ToList();
        }

        //TODO: Remove this when API is implemented
        //To work around incomplete API implementation - load data from resource
        private HttpResponseMessage CreateWorkaroundResponse(string resource = "CourseDirectoryTLevelDetailResponse")
        {
            var json = $"{Assembly.GetExecutingAssembly().GetName().Name}.Data.{resource}.json"
                .ReadManifestResourceStreamAsString();

            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json)
            };
        }

        private IList<Provider> ProcessTLevelDetailsDocument(JsonDocument jsonDoc)
        {
            var providers = new List<Provider>();

            foreach (var courseElement in jsonDoc.RootElement
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
                        Id = providers.Count + 1,
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
                .EnumerateArray()
                .Select(q =>
                    new Qualification
                    {
                        Id = q.SafeGetInt32("frameworkCode"),
                        Name = Regex.Replace(
                            q.SafeGetString("name"),
                                $"^{QualificationTitlePrefix}", "")
                    }).ToList();
        }

        private async Task<(int Saved, int Deleted)> UpdateProvidersInTableStorage(IList<Provider> providers)
        {
            //var removedProviders = await _tableStorageService.ClearProviders();
            //_logger.LogInformation($"Removed {removedProviders} providers from table storage");
            
            var existingProviders = await _tableStorageService.GetAllProviders();

            var providersToInsertOrUpdate = providers.Where(p =>
                    existingProviders.All(x => x.UkPrn != p.UkPrn) //Not in existing data, so add it
                    //TODO: Need a full comparison here - add a comparer
                    || existingProviders.Any(x => x.Id == p.Id && x.Name != p.Name) //Is in existing data and has changed
            ).ToList();

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
            //var removedQualifications = await _tableStorageService.ClearQualifications();
            //_logger.LogInformation($"Removed {removedQualifications} qualifications from table storage");

            var existingQualifications = await _tableStorageService.GetAllQualifications();
            
            var qualificationsToInsertOrUpdate = qualifications.Where(q =>
                    existingQualifications.All(x => x.Id != q.Id) //Not in existing data, so add it
                    || existingQualifications.Any(x => x.Id == q.Id && x.Name != q.Name) //Is in existing data and has changed
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

        private static int MapQualificationId(int id)
        {
            return id switch
            {
                41 => 1, //Building Services Engineering for Construction"
                36 => 2, //Design, Surveying and Planning for Construction
                40 => 3, //Digital Business Services
                37 => 4, //Digital Production, Design and Development
                39 => 5, //Digital Support Services
                38 => 6, //Education and Childcare
                44 => 7, //Health
                45 => 8, //Healthcare Science
                42 => 9, //Onsite Construction
                43 => 10, //Science
                _ => 0
            };
        }
    }
}
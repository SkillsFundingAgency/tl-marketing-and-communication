using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
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

        public async Task<string> GetJsonFromCourseDirectoryApi()
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

        public async Task<int> ImportFromCourseDirectoryApi()
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
                var content = await response.Content.ReadAsStringAsync();
                content = $"[\n{content}\n]";
                response.Content = new StringContent(content);
            }

            response.EnsureSuccessStatusCode();

            var jsonDoc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
            var providers = await ProcessTLevelDetailsDocument(jsonDoc);

            return await UpdateProvidersInTableStorage(providers);
        }

        //TODO: Remove this when API is implemented
        //To work around incomplete API implementation - load data from resource
        private HttpResponseMessage CreateWorkaroundResponse()
        {
            var json = $"{GetType().Namespace}.Data.CourseDirectoryTLevelDetailResponse.json"
                .ReadManifestResourceStreamAsString();

            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json)
            };
        }

        private async Task<IList<Provider>> ProcessTLevelDetailsDocument(JsonDocument jsonDoc)
        {
            var providers = new List<Provider>();

            foreach (var courseRecord in jsonDoc.RootElement
                .EnumerateArray()
                .Where(courseRecord => courseRecord.SafeGetString("offeringType") == "TLevel"))
            {
                var provider = await ProcessCourseRecord(courseRecord);
                if (provider != null)
                {
                    var existingProvider = providers.SingleOrDefault(p => p.UkPrn == provider.UkPrn);
                    if (existingProvider == null)
                    {
                        //TODO: Should we use this as id? Or use UKPRN?
                        provider.Id = providers.Count + 1;
                        providers.Add(provider);
                    }
                    else
                    {
                        //Merge provider
                    }
                }
            }

            return providers;
        }

        private async Task<Provider> ProcessCourseRecord(JsonElement courseRecord)
        {
            _logger.LogWarning($"Processing T Level with id '{courseRecord.SafeGetString("tLevelId")}'");

            if (!DateTime.TryParse(courseRecord.SafeGetString("startDate"), out var startDate))
            {
                //TODO: What to do here? If no date should probably log an error and/or ignore this record
                _logger.LogWarning("Could not read start date from course record.");
                return null;
            }

            //Read qualification
            var qualificationId = 0;
            if (courseRecord.TryGetProperty("qualification", out var qualificationProperty))
            {
                qualificationId = MapQualificationId(qualificationProperty.SafeGetInt32("frameworkCode"));
                if (qualificationId == 0)
                {
                    _logger.LogWarning("Could not find qualification.");
                    return null;
                }
            }

            //Read provider
            if (courseRecord.TryGetProperty("provider", out var providerProperty))
            {
                var providerWebsite = providerProperty.SafeGetString("website");

                var provider = new Provider
                {
                    Name = providerProperty.SafeGetString("providerName"),
                    UkPrn = int.TryParse(providerProperty.SafeGetString("ukprn"), out var ukPrn) ? ukPrn : 0,
                    
                    Locations = courseRecord.TryGetProperty("locations", out var locationsProperty)
                        ? locationsProperty.EnumerateArray().Select(l =>
                        {
                            var locationWebsite = l.SafeGetString("website");

                            return new Location
                            {
                                Name = l.SafeGetString("venueName"),
                                Postcode = l.SafeGetString("postcode"),
                                Town = l.SafeGetString("town"),
                                Latitude = l.SafeGetDouble("latitude"),
                                Longitude = l.SafeGetDouble("longitude"),
                                //TODO: Should website be from venue or provider, or both
                                //Website = !string.IsNullOrWhiteSpace(l.SafeGetString("website"))
                                //    ? l.SafeGetString("website")
                                //    : providerProperty.SafeGetString("website"),
                                Website = !string.IsNullOrWhiteSpace(locationWebsite)
                                    ? locationWebsite   
                                    : providerWebsite,
                                DeliveryYears = new List<DeliveryYearDto>
                                {
                                    new DeliveryYearDto
                                    {
                                        Year = (short)startDate.Year,
                                        Qualifications = new List<int>
                                        {
                                            qualificationId
                                        }
                                    }
                                }
                            };
                        }).ToList()
                        : new List<Location>()
                };
                
                return provider;
            }

            return null;
        }

        private static int MapQualificationId(int id)
        {
            //Qualifications will have a prefix of "T Level Technical Qualification in "
            //TODO: Replace this with the correct ids returned in the real API - might be a guid or a map to qualifications
            return id switch
            {
                36 => 2, //Design, Surveying and Planning for Construction
                37 => 4, //Digital Production, Design and Development
                38 => 6, //Education and Childcare
                _ => 0
            };
        }

        private async Task<int> UpdateProvidersInTableStorage(IList<Provider> providers)
        {
            //After accumulating all providers
            //TODO: Merge data, don't clear
            var removedProviders = await _tableStorageService.ClearProviders();
            _logger.LogInformation($"Removed {removedProviders} providers from table storage");

            var savedProviders = await _tableStorageService.SaveProviders(providers);
            _logger.LogInformation($"Saved {savedProviders} providers to table storage");

            //TODO: Delete any providers that aren't in the incoming list

            _logger.LogInformation($"ImportFromCourseDirectoryApi saved {providers.Count} records");

            return providers.Count;
        }

        public async Task<IList<Provider>> GetProviders()
        {
            return (await _tableStorageService.RetrieveProviders())
                .OrderBy(p => p.Id).ToList();
        }

        public async Task<IList<Qualification>> GetQualifications()
        {
            return (await _tableStorageService
                .RetrieveQualifications())
                .OrderBy(q => q.Id).ToList();
        }
    }
}
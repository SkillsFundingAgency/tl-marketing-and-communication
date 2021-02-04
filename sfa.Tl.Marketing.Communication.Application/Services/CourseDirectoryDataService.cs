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

            // ReSharper disable once StringLiteralTypo
            var response = await httpClient.GetAsync(CourseDetailEndpoint);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError($"API call failed with {response.StatusCode} - {response.ReasonPhrase}");
            }

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<int> ImportFromCourseDirectoryApi()
        {
            _logger.LogInformation("ImportFromCourseDirectoryApi called");

            var httpClient = _httpClientFactory.CreateClient(CourseDirectoryHttpClientName);

            var response = await httpClient.GetAsync(CourseDetailEndpoint);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError($"API call failed with {response.StatusCode} - {response.ReasonPhrase}");
            }

            //var jsonDoc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
            //TODO: just create the jsonDoc as above - for now wrap the record in an array using string 
            var content = await response.Content.ReadAsStringAsync();
            content = $"[\n{content}\n]";
            var jsonDoc = JsonDocument.Parse(content);

            var root = jsonDoc.RootElement;

            var providers = new List<Provider>();
            foreach (var courseRecord in root.EnumerateArray())
            {
                if (courseRecord.SafeGetString("offeringType")  == "TLevel")
                {
                    var provider = await ProcessCourseRecord(courseRecord);
                    if (provider != null)
                    {
                        var existingProvider = providers.SingleOrDefault(p => p.UkPrn == provider.UkPrn);
                        if (existingProvider == null)
                        {
                            providers.Add(provider);
                        }
                        else
                        {
                            //Merge provider
                        }
                    }
                }
            }

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
                var provider = new Provider
                {
                    Name = providerProperty.SafeGetString("providerName"),
                    UkPrn = int.TryParse(providerProperty.SafeGetString("ukprn"), out var ukPrn) ? ukPrn : 0,
                    
                    Locations = courseRecord.TryGetProperty("locations", out var locationsProperty)
                        ? locationsProperty.EnumerateArray().Select(l =>
                            new Location
                            {
                                Name = l.SafeGetString("venueName"),
                                Postcode = l.SafeGetString("postcode"),
                            }).ToList()
                        : new List<Location>()
                };


                return provider;
            }

            return null;
        }

        private int MapQualificationId(int id)
        {
            return id switch
            {
                36 => 2 //Design, Surveying and Planning for Construction
                ,
                37 => 4 //Digital Production, Design and Development
                ,
                38 => 6 //Education and Childcare
                ,
                _ => 0
            };
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
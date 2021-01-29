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

            var count = 0;
            foreach (var courseRecord in root.EnumerateArray())
            {
                if (courseRecord.SafeGetString("offeringType")  == "TLevel")
                {
                    if(await ProcessCourseRecord(courseRecord))
                        count++;
                }
            }

            _logger.LogInformation($"ImportFromCourseDirectoryApi saved {count} records");

            return count;
        }

        private async Task<bool> ProcessCourseRecord(JsonElement courseRecord)
        {
            //read start date
            if (!DateTime.TryParse(courseRecord.SafeGetString("startDate"), out var startDate))
            {
                //TODO: What to do here? If no date should probably log an error and/or ignore this record
                _logger.LogWarning("Could not read start date from course record.");
                return false;
            }

            //Read qualification
            //Read provider
            var providerProperty = courseRecord.GetProperty("provider");

            var provider = new Provider
            {
                //UkPrn = providerProperty.SafeGetString("ukPrn")
                Name = providerProperty.SafeGetString("providerName"),
                //Locations = TODO - Select from "locations"
            };

            //read locations

            return true;
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
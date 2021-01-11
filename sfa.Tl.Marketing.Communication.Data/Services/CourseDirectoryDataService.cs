using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using sfa.Tl.Marketing.Communication.Data.Interfaces;

namespace sfa.Tl.Marketing.Communication.Data.Services
{
    public class CourseDirectoryDataService : ICourseDirectoryDataService
    {
        public const string CourseDirectoryHttpClientName = "CourseDirectoryAutoCompressClient";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<CourseDirectoryDataService> _logger;

        public CourseDirectoryDataService(IHttpClientFactory httpClientFactory, ILogger<CourseDirectoryDataService> logger)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> ImportFromCourseDirectoryApi()
        {
            _logger.LogInformation("ImportFromCourseDirectoryApi called");

            var httpClient = _httpClientFactory.CreateClient(CourseDirectoryHttpClientName);

            //https://dev.api.nationalcareersservice.org.uk/coursedirectory/findacourse/tleveldetail[?TLevelId]
            // ReSharper disable once StringLiteralTypo
            var response = await httpClient.GetAsync("tleveldetail");
            if (response.StatusCode != HttpStatusCode.OK)
            {
                //TODO: Add logger
                Console.WriteLine($"Response failed with {response.StatusCode} - {response.ReasonPhrase}");
            }

            //var content = await response.Content.ReadAsStringAsync();
            var jsonDoc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
            var root = jsonDoc.RootElement;

            //Should always check "offeringType": "TLevel"
            string offeringType = null;
            if (root.TryGetProperty("offeringType", out var offeringTypeElement))
            {
                offeringType = offeringTypeElement.GetString();
            }

            Console.WriteLine($"offeringType: {offeringType}");

            //For the initial version we just need to confirm 1 record was found. This will change before go-live
            //Should count json records, or records saved
            var count = offeringType == "TLevel" ? 1 : 0;

            _logger.LogInformation($"ImportFromCourseDirectoryApi saved {count} records");

            return count;
        }
    }
}
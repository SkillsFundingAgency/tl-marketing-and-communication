using sfa.Tl.Marketing.Communication.Application.GeoLocations;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Application.Services;
using sfa.Tl.Marketing.Communication.Models.Configuration;
using sfa.Tl.Marketing.Communication.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace sfa.Tl.Marketing.Communication.IntegrationTests
{
    public class ProviderUrlTests
    {
        private readonly IProviderSearchService _providerSearchService;
        private readonly ITestOutputHelper _outputHelper;

        public ProviderUrlTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;

            var configurationOptions = new ConfigurationOptions
            {
                PostcodeRetrieverBaseUrl = @"http://postcode.io.uk"
            };

            var tableStorageService = Substitute.For<ITableStorageService>();
            var loggerFactory = new LoggerFactory();
            var logger = loggerFactory.CreateLogger<ProviderDataService>();

            var providerDataService = new ProviderDataService(tableStorageService, logger);
            var locationService = new LocationService();
            var journeyService = new JourneyService();
            var providerLocationService = new ProviderLocationService(providerDataService);
            var distanceCalculationService = new DistanceCalculationService(new LocationApiClient(new HttpClient(), configurationOptions));
            _providerSearchService = new ProviderSearchService(providerDataService, journeyService, locationService, providerLocationService, distanceCalculationService);
        }

        [Fact]
        public async Task Check_If_a_Provider_Website_Is_Broken()
        {
            var locations = _providerSearchService.GetAllProviderLocations().ToList();
            var locationsWithBrokenUrls = new List<ProviderLocation>();

            foreach (var location in locations)
            {
                var isUrlBroken = await IsUrlBroken(location.Website);
                if (isUrlBroken)
                {
                    locationsWithBrokenUrls.Add(location);
                }
            }

            if (locationsWithBrokenUrls.Any())
            {
                _outputHelper.WriteLine($"\n{locationsWithBrokenUrls.Count} out of {locations.Count} Providers has broken Websites, as shown below:\n");
                
                var providersWithBrokenUrls = from providerLocation in locationsWithBrokenUrls
                                   select new { providerLocation.ProviderName, providerLocation.Website };
                var json = SerializeObject(providersWithBrokenUrls);
                
                _outputHelper.WriteLine(json);

                Assert.True(false, $"There are {locationsWithBrokenUrls.Count} out of {locations.Count} Providers with broken websites.");
            }
            else
            {
                var successMessage = $"All {locations.Count} providers websites are working fine.";
                _outputHelper.WriteLine(successMessage);
                Assert.True(true, successMessage);
            }
        }

        private async Task<bool> IsUrlBroken(string url)
        {
            var isUrlBroken = false;

            try
            {
                var clientHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = 
                        (sender, cert, chain, sslPolicyErrors) 
                            => true
                };

                using var client = new HttpClient(clientHandler)
                {
                    Timeout = TimeSpan.FromSeconds(10)
                };

                var checkingResponse = await client.GetAsync(url);
                if (checkingResponse.StatusCode == HttpStatusCode.NotFound)
                {

                    isUrlBroken = true;
                }
            }
            catch(Exception ex)
            {
                _outputHelper.WriteLine($"\n\rWebsite: {url}\nError message: {ex.Message}\nStackTrace: {ex.StackTrace}\n");
            }

            return isUrlBroken;
        }

        public string SerializeObject(object data)
        {
            var serializerOptions = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(data, serializerOptions);
            return json;
        }
    }
}

using sfa.Tl.Marketing.Communication.Application.GeoLocations;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Application.Services;
using sfa.Tl.Marketing.Communication.Models.Configuration;
using sfa.Tl.Marketing.Communication.Models.Dto;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace sfa.Tl.Marketing.Communication.IntegrationTests
{
    public class ProviderUrlTests
    {
        private readonly IProviderSearchService _providerSearchService;
        private readonly ITestOutputHelper _outputHelper;
        private readonly IJsonConvertor _jsonConvertor;

        public ProviderUrlTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;

            var appDomain = System.AppDomain.CurrentDomain;
            var basePath = appDomain.RelativeSearchPath ?? appDomain.BaseDirectory;
            var dataFilePath = Path.Combine(basePath, "Data", "providers.json");
            var configurationOptions = new ConfigurationOptions()
            {
                DataFilePath = dataFilePath,
                PostcodeRetrieverBaseUrl = @"http://poscode.io.uk"
            };

            var fileReader = new FileReader();
            _jsonConvertor = new JsonConvertor();
            var providerDataService = new ProviderDataService(fileReader, _jsonConvertor, configurationOptions);
            var locationService = new LocationService();
            var providerLocationService = new ProviderLocationService(providerDataService);
            var distanceCalculationService = new DistanceCalculationService(new LocationApiClient(new HttpClient(), configurationOptions), new DistanceService());
            _providerSearchService = new ProviderSearchService(providerDataService, locationService, providerLocationService, distanceCalculationService);
        }

        [Fact]
        public async Task Check_If_a_Provider_Website_Is_Broken()
        {
            var locations = _providerSearchService.GetAllProviderLocations();
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
                _outputHelper.WriteLine($"\n{locationsWithBrokenUrls.Count()} out of {locations.Count()} Providers has broken Websites, as shown below:\n");
                
                var providersWithBrokenUrls = from providerLocation in locationsWithBrokenUrls
                                   select new { providerLocation.ProviderName, providerLocation.Website };
                var json = _jsonConvertor.SerializeObject(providersWithBrokenUrls);
                
                _outputHelper.WriteLine(json);

                Assert.True(false, $"There are {locationsWithBrokenUrls.Count()} out of {locations.Count()} Providers with broken websites.");
            }
            else
            {
                var successMessage = $"All {locations.Count()} providers websites are working fine.";
                _outputHelper.WriteLine(successMessage);
                Assert.True(true, successMessage);
            }
        }

        private async Task<bool> IsUrlBroken(string url)
        {
            bool isUrlBroken = false;

            try
            {
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                
                using (var client = new HttpClient(clientHandler))
                {
                    client.Timeout = TimeSpan.FromSeconds(10);
                    var checkingResponse = await client.GetAsync(url);
                    if (checkingResponse.StatusCode == HttpStatusCode.NotFound)
                    {

                        isUrlBroken = true;
                    }
                }
            }
            catch(Exception ex)
            {
                _outputHelper.WriteLine($"\n\rWebsite: {url}\nError message: {ex.Message}\nStackTrace: {ex.StackTrace}\n");
            }

            return isUrlBroken;
        }
    }
}

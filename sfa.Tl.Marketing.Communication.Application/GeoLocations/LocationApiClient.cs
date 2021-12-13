using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using sfa.Tl.Marketing.Communication.Application.Extensions;
using sfa.Tl.Marketing.Communication.Models.Configuration;
using sfa.Tl.Marketing.Communication.Models.Dto;
using sfa.Tl.Marketing.Communication.Models.Extensions;

namespace sfa.Tl.Marketing.Communication.Application.GeoLocations
{
    public class LocationApiClient : ILocationApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly Uri _postcodeRetrieverBaseUri;

        public const double DefaultLatitude = 51.477928;
        public const double DefaultLongitude = 0;

        public LocationApiClient(HttpClient httpClient, ConfigurationOptions configurationOptions)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (string.IsNullOrWhiteSpace(configurationOptions.PostcodeRetrieverBaseUrl) ||
                !Uri.TryCreate(configurationOptions.PostcodeRetrieverBaseUrl, UriKind.Absolute, out _postcodeRetrieverBaseUri))
            {
                throw new ArgumentException("PostcodeRetrieverBaseUrl configuration is not set or is not a valid URI. Please check the application settings.");
            }
        }

        public async Task<PostcodeLookupResultDto> GetGeoLocationDataAsync(string postcode)
        {
            var lookupUri = new Uri(_postcodeRetrieverBaseUri, $"postcodes/{postcode.ToLetterOrDigit()}");

            var responseMessage = await _httpClient.GetAsync(lookupUri);

            return responseMessage.StatusCode == HttpStatusCode.OK
                ? await ReadPostcodeLocationFromResponse(responseMessage)
                : await GetTerminatedPostcodeGeoLocationDataAsync(postcode);
        }
        
        private async Task<PostcodeLookupResultDto> GetTerminatedPostcodeGeoLocationDataAsync(string postcode)
        {
            var lookupUri = new Uri(_postcodeRetrieverBaseUri, $"terminated_postcodes/{postcode.ToLetterOrDigit()}");

            var responseMessage = await _httpClient.GetAsync(lookupUri);

            return responseMessage.StatusCode == HttpStatusCode.OK
                ? await ReadPostcodeLocationFromResponse(responseMessage)
                : null;
        }

        private static async Task<PostcodeLookupResultDto> ReadPostcodeLocationFromResponse(HttpResponseMessage responseMessage)
        {
            using var jsonDocument = await JsonDocument.ParseAsync(await responseMessage.Content.ReadAsStreamAsync());
            var resultElement = jsonDocument.RootElement.GetProperty("result");
            
            return new PostcodeLookupResultDto
            {
                Postcode = resultElement.SafeGetString("postcode"),
                Latitude = resultElement.SafeGetDouble("latitude", DefaultLatitude),
                Longitude = resultElement.SafeGetDouble("longitude")
            };
        }
    }
}
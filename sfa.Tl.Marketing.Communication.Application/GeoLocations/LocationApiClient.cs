using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using sfa.Tl.Marketing.Communication.Models.Configuration;
using sfa.Tl.Marketing.Communication.Models.Dto;
using sfa.Tl.Marketing.Communication.Models.Extensions;

namespace sfa.Tl.Marketing.Communication.Application.GeoLocations
{
    public class LocationApiClient : ILocationApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly Uri _postcodeRetrieverBaseUri;

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

            if (responseMessage.StatusCode == HttpStatusCode.OK)
            {
                var response = await responseMessage.Content.ReadAsAsync<PostcodeLookupResponse>();
                return response.Result;
            }

            return await GetTerminatedPostcodeGeoLocationDataAsync(postcode);
        }

        private async Task<PostcodeLookupResultDto> GetTerminatedPostcodeGeoLocationDataAsync(string postcode)
        {
            var lookupUri = new Uri(_postcodeRetrieverBaseUri, $"terminated_postcodes/{postcode.ToLetterOrDigit()}");

            var responseMessage = await _httpClient.GetAsync(lookupUri);

            if (responseMessage.StatusCode == HttpStatusCode.OK)
            {
                var response = await responseMessage.Content.ReadAsAsync<PostcodeLookupResponse>();
                return response.Result;
            }

            return null;
        }
    }
}
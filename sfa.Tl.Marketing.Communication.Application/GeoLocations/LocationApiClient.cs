using System;
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

        public async Task<(bool, string)> IsValidPostcodeAsync(string postcode, bool includeTerminated)
        {
            var (isValidPostcode, postcodeResult) = await IsValidPostcodeAsync(postcode);
            if (!isValidPostcode)
            {
                (isValidPostcode, postcodeResult) = await IsTerminatedPostcodeAsync(postcode);
            }

            return (isValidPostcode, postcodeResult);
        }

        public async Task<(bool, string)> IsValidPostcodeAsync(string postcode)
        {
            try
            {
                var postcodeLookupResultDto = await GetGeoLocationDataAsync(postcode);
                return (true, postcodeLookupResultDto.Postcode);
            }
            catch
            {
                return (false, string.Empty);
            }
        }

        public async Task<(bool, string)> IsTerminatedPostcodeAsync(string postcode)
        {
            try
            {
                var postcodeLookupResultDto = await GetTerminatedPostcodeGeoLocationDataAsync(postcode);
                return (true, postcodeLookupResultDto.Postcode);
            }
            catch
            {
                return (false, string.Empty);
            }
        }

        public async Task<PostcodeLookupResultDto> GetGeoLocationDataAsync(string postcode, bool includeTerminated)
        {
            try
            {
                return await GetGeoLocationDataAsync(postcode);
            }
            catch
            {
                if (!includeTerminated)
                    throw;
            }

            return await GetTerminatedPostcodeGeoLocationDataAsync(postcode);
        }

        public async Task<PostcodeLookupResultDto> GetGeoLocationDataAsync(string postcode)
        {
            //Postcodes.io Returns 404 for "CV12 wt" so I have removed all special characters to get best possible result
            var lookupUri = new Uri(_postcodeRetrieverBaseUri, $"postcodes/{postcode.ToLetterOrDigit()}");

            var responseMessage = await _httpClient.GetAsync(lookupUri);
            
            responseMessage.EnsureSuccessStatusCode();

            var response = await responseMessage.Content.ReadAsAsync<PostcodeLookupResponse>();

            return response.Result;
        }

        public async Task<PostcodeLookupResultDto> GetTerminatedPostcodeGeoLocationDataAsync(string postcode)
        {
            var lookupUri = new Uri(_postcodeRetrieverBaseUri, $"terminated_postcodes/{postcode.ToLetterOrDigit()}");

            var responseMessage = await _httpClient.GetAsync(lookupUri);

            responseMessage.EnsureSuccessStatusCode();

            var response = await responseMessage.Content.ReadAsAsync<PostcodeLookupResponse>();

            return response.Result;
        }
    }
}
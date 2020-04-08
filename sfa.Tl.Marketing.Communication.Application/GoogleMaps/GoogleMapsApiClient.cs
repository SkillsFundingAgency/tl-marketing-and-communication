using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using sfa.Tl.Marketing.Communication.Application.Extensions;
using sfa.Tl.Marketing.Communication.Models.Configuration;

namespace sfa.Tl.Marketing.Communication.Application.GoogleMaps
{
    public class GoogleMapApiClient : IGoogleMapApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ConfigurationOptions _configuration;

        public GoogleMapApiClient(HttpClient httpClient, ConfigurationOptions configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public Task<int> ComputeDistanceBetweenInMiles(double lat1, double lon1, double lat2, double lon2)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetAddressDetailsAsync(string postcode)
        {
            if (string.IsNullOrWhiteSpace(_configuration.GoogleMapsApiKey)) return null;
            
            var lookupUrl = $"{_configuration.GoogleMapsApiBaseUrl}place/textsearch/json?region=uk&radius=1&key={_configuration.GoogleMapsApiKey}&query={postcode.ToLetterOrDigit()}";

            var responseMessage = await _httpClient.GetAsync(lookupUrl);

            responseMessage.EnsureSuccessStatusCode();

            var response = await responseMessage.Content.ReadAsAsync<GooglePlacesResult>();
            
            //Google return "StreetName, Town Postcode" therefore below , Please note this will not work if input postcode is in lowercase and or does not have Space between segments
            return response.Status != "OK" ? string.Empty : response.Results.First().FormattedAddress.Split(",").Last().Replace(postcode, string.Empty).Trim();
        }
    }

    public class GooglePlacesResult
    {
        [JsonProperty("html_attributions")]
        public object[] HtmlAttributions { get; set; }

        [JsonProperty("results")]
        public Result[] Results { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public class Result
    {
        [JsonProperty("formatted_address")]
        public string FormattedAddress { get; set; }

        [JsonProperty("geometry")]
        public Geometry Geometry { get; set; }

        [JsonProperty("icon")]
        public Uri Icon { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("place_id")]
        public string PlaceId { get; set; }

        [JsonProperty("reference")]
        public string Reference { get; set; }

        [JsonProperty("types")]
        public string[] Types { get; set; }
    }

    public class Geometry
    {
        [JsonProperty("location")]
        public Location Location { get; set; }

        [JsonProperty("viewport")]
        public Viewport Viewport { get; set; }
    }

    public class Location
    {
        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lng")]
        public double Lng { get; set; }
    }

    public class Viewport
    {
        [JsonProperty("northeast")]
        public Location Northeast { get; set; }

        [JsonProperty("southwest")]
        public Location Southwest { get; set; }
    }

}
using FluentAssertions;
using sfa.Tl.Marketing.Communication.Application.GeoLocations;
using sfa.Tl.Marketing.Communication.Models.Configuration;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using sfa.Tl.Marketing.Communication.UnitTests.Builders;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Application.GeoLocations
{
    public class LocationApiClientUnitTests
    {
        private readonly ConfigurationOptions _configurationOptions;

        private const string GoodPostcode = "CV1 2WT";
        private const string TerminatedPostcode = "S70 2YW";
        private const string NoLatLongPostcode = "GY1 4NS";

        public LocationApiClientUnitTests()
        {
            _configurationOptions = new ConfigurationOptions
            {
                PostcodeRetrieverBaseUrl = "https://example.com/"
            };
        }

        [Fact]
        public async Task Then_Postcode_Is_Returned_Correctly()
        {
            var httpClient = IntializeHttpClient(GoodPostcode);

            var locationApiClient = new LocationApiClient(httpClient, _configurationOptions);

            var postcodeData = await locationApiClient.GetGeoLocationDataAsync(GoodPostcode);

            postcodeData.Should().NotBeNull();
            postcodeData.Postcode.Should().Be(GoodPostcode);
            postcodeData.Latitude.Should().Be(52.400997);
            postcodeData.Longitude.Should().Be(-1.508122);
        }

        [Fact]
        public async Task Then_Terminated_Postcode_Is_Returned_Correctly()
        {
            var httpClient = IntializeTerminatedHttpClient(TerminatedPostcode);

            var locationApiClient = new LocationApiClient(httpClient, _configurationOptions);

            var postcodeData = await locationApiClient
                .GetGeoLocationDataAsync(TerminatedPostcode);

            postcodeData.Should().NotBeNull();
            postcodeData.Postcode.Should().Be(TerminatedPostcode);
            postcodeData.Latitude.Should().Be(53.551618);
            postcodeData.Longitude.Should().Be(-1.482797);
        }

        [Fact]
        public async Task Then_Postcode_With_No_Lat_Long_Is_Returned_Correctly()
        {
            var httpClient = IntializeHttpClient(NoLatLongPostcode);

            var locationApiClient = new LocationApiClient(httpClient, _configurationOptions);

            var postcodeData = await locationApiClient.GetGeoLocationDataAsync(NoLatLongPostcode);

            postcodeData.Should().NotBeNull();
            postcodeData.Postcode.Should().Be(NoLatLongPostcode);
            postcodeData.Latitude.Should().Be(LocationApiClient.DefaultLatitude);
            postcodeData.Longitude.Should().Be(LocationApiClient.DefaultLongitude);
        }

        private static HttpClient IntializeHttpClient(string requestPostcode)
        {
            var json = new PostcodeResponseJsonBuilder().BuildValidPostcodeResponse(requestPostcode);
            return CreateClient(json, $"https://example.com/terminated_postcodes/{requestPostcode.Replace(" ", "")}");
        }

        private static HttpClient IntializeTerminatedHttpClient(string requestPostcode)
        {
            var json = new PostcodeResponseJsonBuilder().BuildVTerminatedPostcodeResponse(requestPostcode);
            return CreateClient(json, $"https://example.com/terminated_postcodes/{requestPostcode.Replace(" ", "")}");
        }

        private static HttpClient CreateClient(string json, string uri, string contentType = "application/json")
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json)
            };
            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

            var fakeMessageHandler = new FakeHttpMessageHandler();
            fakeMessageHandler.AddFakeResponse(new Uri(uri),
                httpResponseMessage);

            var httpClient = new HttpClient(fakeMessageHandler);

            return httpClient;
        }
    }
}

using FluentAssertions;
using sfa.Tl.Marketing.Communication.Application.GeoLocations;
using sfa.Tl.Marketing.Communication.Models.Configuration;
using sfa.Tl.Marketing.Communication.Models.Dto;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Application.GeoLocations
{
    public class LocationApiClientUnitTests
    {
        private readonly ConfigurationOptions _configurationOptions;

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
            // Arrange
            var responseData = new PostcodeLookupResultDto
            {
                Postcode = "CV1 2WT",
                Latitude = 50.001,
                Longitude = -1.234
            };

            var httpClient = IntializeHttpClient("CV1 2WT", responseData);

            var locationApiClient = new LocationApiClient(httpClient, _configurationOptions);
            
            // Act
            var postcodeData = await locationApiClient.GetGeoLocationDataAsync("CV12WT");

            // Assert
            postcodeData.Should().NotBeNull();
            postcodeData.Postcode.Should().Be("CV1 2WT");
            postcodeData.Latitude.Should().Be(50.001);
            postcodeData.Longitude.Should().Be(-1.234);
        }

        [Fact]
        public async Task Then_Terminated_Postcode_Is_Returned_Correctly()
        {
            // Arrange
            var responseData = new PostcodeLookupResultDto
            {
                Postcode = "S70 2YW",
                Latitude = 50.001,
                Longitude = -1.234
            };

            var httpClient = IntializeTerminatedHttpClient("S70 2YW", responseData);

            var locationApiClient = new LocationApiClient(httpClient, _configurationOptions);
            
            // Act
            var postcodeData = await locationApiClient
                .GetGeoLocationDataAsync("S702YW");

            // Assert
            postcodeData.Should().NotBeNull();
            postcodeData.Postcode.Should().Be("S70 2YW");
            postcodeData.Latitude.Should().Be(50.001);
            postcodeData.Longitude.Should().Be(-1.234);
        }

        private static HttpClient IntializeHttpClient(string requestPostcode, PostcodeLookupResultDto responseData)
        {
            var response = new PostcodeLookupResponse
            {
                Result = responseData,
                Status = 200
            };

            return CreateClient(response, $"https://example.com/postcodes/{requestPostcode.Replace(" ", "")}");
        }

        private static HttpClient IntializeTerminatedHttpClient(string requestPostcode, PostcodeLookupResultDto responseData)
        {
            var response = new PostcodeLookupResponse
            {
                Result = responseData,
                Status = 200
            };

            return CreateClient(response, $"https://example.com/terminated_postcodes/{requestPostcode.Replace(" ", "")}");
        }

        private static HttpClient CreateClient(object response, string uri, string contentType = "application/json")
        {
            var json = JsonSerializer.Serialize(response);

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

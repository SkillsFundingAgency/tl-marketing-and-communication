using FluentAssertions;
using sfa.Tl.Marketing.Communication.Application.GeoLocations;
using sfa.Tl.Marketing.Communication.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using sfa.Tl.Marketing.Communication.UnitTests.Builders;
using sfa.Tl.Marketing.Communication.UnitTests.TestHelpers.HttpClientHelpers;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Application.GeoLocations
{
    public class LocationApiClientUnitTests
    {
        private readonly ConfigurationOptions _configurationOptions;

        private const string BaseUrl = "https://test.postcodes.com/";
        private const string GoodPostcode = "CV1 2WT";
        private const string InvalidPostcode = "CVX XXX";
        private const string Outcode = "CV11";
        private const string TerminatedPostcode = "S70 2YW";
        private const string NoLatLongPostcode = "GY1 4NS";

        public LocationApiClientUnitTests()
        {
            _configurationOptions = new ConfigurationOptions
            {
                PostcodeRetrieverBaseUrl = BaseUrl
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
        public async Task Then_Outcode_Is_Returned_Correctly()
        {
            var httpClient = IntializeOutcodeHttpClient(Outcode);

            var locationApiClient = new LocationApiClient(httpClient, _configurationOptions);

            var postcodeData = await locationApiClient
                .GetGeoLocationDataAsync(Outcode);

            postcodeData.Should().NotBeNull();
            postcodeData.Postcode.Should().Be(Outcode);
            postcodeData.Latitude.Should().Be(52.5198364972316);
            postcodeData.Longitude.Should().Be(-1.45313659025471);
        }

        [Fact]
        public async Task Then_Invalid_Postcode_Returns_Null()
        {
            var jsonBuilder = new PostcodeResponseJsonBuilder();

            var httpClient = new TestHttpClientFactory()
                .CreateClient(
                    new List<(Uri, string, HttpStatusCode)>
                    {
                        (new Uri($"{BaseUrl}postcodes/{InvalidPostcode}"),
                            jsonBuilder.BuildPostcodeNotFoundResponse(),
                            HttpStatusCode.NotFound),
                        (new Uri($"{BaseUrl}terminated_postcodes/{InvalidPostcode}"),
                            jsonBuilder.BuildPostcodeNotFoundResponse(),
                            HttpStatusCode.NotFound),
                        (new Uri($"{BaseUrl}outcodes/{InvalidPostcode}"),
                            jsonBuilder.BuildPostcodeNotFoundResponse(),
                            HttpStatusCode.NotFound)
                    });

            var locationApiClient = new LocationApiClient(httpClient, _configurationOptions);

            var postcodeData = await locationApiClient
                .GetGeoLocationDataAsync(InvalidPostcode);

            postcodeData.Should().BeNull();
        }

        [Fact]
        public async Task Then_Invalid_Outcode_Returns_Null()
        {
            var httpClient = new TestHttpClientFactory().CreateHttpClient(
                new Uri($"{BaseUrl}outcodes/{Outcode}"),
                new PostcodeResponseJsonBuilder().BuildPostcodeNotFoundResponse(),
                responseCode: HttpStatusCode.NotFound);

            var locationApiClient = new LocationApiClient(httpClient, _configurationOptions);

            var postcodeData = await locationApiClient
                .GetGeoLocationDataAsync(Outcode);

            postcodeData.Should().BeNull();
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
            return new TestHttpClientFactory().CreateHttpClient(
                new Uri($"{BaseUrl}postcodes/{requestPostcode.Replace(" ", "")}"),
                new PostcodeResponseJsonBuilder().BuildValidPostcodeResponse(requestPostcode));
        }

        private static HttpClient IntializeTerminatedHttpClient(string requestPostcode)
        {
            return new TestHttpClientFactory().CreateHttpClient(
                new Uri($"{BaseUrl}terminated_postcodes/{requestPostcode.Replace(" ", "")}"),
                new PostcodeResponseJsonBuilder().BuildTerminatedPostcodeResponse(requestPostcode));
        }

        private static HttpClient IntializeOutcodeHttpClient(string requestPostcode)
        {
            return new TestHttpClientFactory().CreateHttpClient(
                new Uri($"{BaseUrl}outcodes/{requestPostcode.Replace(" ", "")}"),
                new PostcodeResponseJsonBuilder().BuildOutcodeResponse(requestPostcode));
        }
    }
}

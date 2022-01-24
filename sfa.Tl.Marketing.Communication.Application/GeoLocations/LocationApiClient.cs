using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using sfa.Tl.Marketing.Communication.Application.Extensions;
using sfa.Tl.Marketing.Communication.Models.Dto;
using sfa.Tl.Marketing.Communication.Models.Extensions;

namespace sfa.Tl.Marketing.Communication.Application.GeoLocations;

public class LocationApiClient : ILocationApiClient
{
    private readonly HttpClient _httpClient;

    public const double DefaultLatitude = 51.477928;
    public const double DefaultLongitude = 0;

    public LocationApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<PostcodeLookupResultDto> GetGeoLocationDataAsync(string postcode)
    {
        var formattedPostcode = postcode.ToLetterOrDigit();

        if (formattedPostcode.Length <= 4)
        {
            return await GetOutcode(formattedPostcode);
        }

        var responseMessage = await _httpClient.GetAsync(
            $"postcodes/{formattedPostcode}");

        return responseMessage.StatusCode == HttpStatusCode.OK
            ? await ReadPostcodeLocationFromResponse(responseMessage)
            : await GetTerminatedPostcodeGeoLocationDataAsync(formattedPostcode);
    }
        
    private async Task<PostcodeLookupResultDto> GetTerminatedPostcodeGeoLocationDataAsync(string formattedPostcode)
    {
        var responseMessage = await _httpClient.GetAsync(
            $"terminated_postcodes/{formattedPostcode}");

        return responseMessage.StatusCode == HttpStatusCode.OK
            ? await ReadPostcodeLocationFromResponse(responseMessage)
            : null;
    }

    public async Task<PostcodeLookupResultDto> GetOutcode(string formattedOutcode)
    {
        var responseMessage = await _httpClient.GetAsync(
            $"outcodes/{formattedOutcode}");

        return responseMessage.StatusCode == HttpStatusCode.OK
            ? await ReadPostcodeLocationFromResponse(responseMessage,
                "outcode")
            : null;
    }

    private static async Task<PostcodeLookupResultDto> ReadPostcodeLocationFromResponse(
        HttpResponseMessage responseMessage,
        string postcodeFieldName = "postcode")
    {
        using var jsonDocument = await JsonDocument.ParseAsync(await responseMessage.Content.ReadAsStreamAsync());
        var resultElement = jsonDocument.RootElement.GetProperty("result");
            
        return new PostcodeLookupResultDto
        {
            Postcode = resultElement.SafeGetString(postcodeFieldName),
            Latitude = resultElement.SafeGetDouble("latitude", DefaultLatitude),
            Longitude = resultElement.SafeGetDouble("longitude")
        };
    }
}
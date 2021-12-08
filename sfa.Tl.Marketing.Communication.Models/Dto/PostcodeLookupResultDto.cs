using System.Text.Json.Serialization;

namespace sfa.Tl.Marketing.Communication.Models.Dto
{
    public class PostcodeLookupResultDto
    {
        [JsonPropertyName("postcode")]
        public string Postcode { get; init; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; init; }

        [JsonPropertyName("latitude")]
        public double Latitude { get; init; }
    }
}
using System.Text.Json.Serialization;

namespace sfa.Tl.Marketing.Communication.Models.Dto
{
    public class PostcodeLookupResultDto
    {
        [JsonPropertyName("postcode")]
        public string Postcode { get; set; }
        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }
    }
}
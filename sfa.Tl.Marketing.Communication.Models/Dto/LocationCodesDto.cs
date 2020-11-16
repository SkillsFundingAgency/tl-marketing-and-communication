using System.Text.Json.Serialization;

namespace sfa.Tl.Marketing.Communication.Models.Dto
{
    public class LocationCodesDto
    {
        [JsonPropertyName("admin_district")]
        public string AdminDistrict { get; set; }

        [JsonPropertyName("admin_county")]
        public string AdminCounty { get; set; }

        [JsonPropertyName("admin_ward")]
        public string AdminWard { get; set; }

        [JsonPropertyName("parish")]
        public string Parish { get; set; }

        [JsonPropertyName("parliamentary_constituency")]
        public string ParliamentaryConstituency { get; set; }

        [JsonPropertyName("ccg")]
        public string Ccg { get; set; }

        [JsonPropertyName("ced")]
        public string Ced { get; set; }

        [JsonPropertyName("nuts")]
        public string Nuts { get; set; }
    }
}
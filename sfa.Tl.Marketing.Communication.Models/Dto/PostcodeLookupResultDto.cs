using Newtonsoft.Json;

namespace sfa.Tl.Marketing.Communication.Models.Dto
{
    public class PostcodeLookupResultDto
    {
        [JsonProperty("postcode")]
        public string Postcode { get; set; }
        [JsonProperty("longitude")]
        public string Longitude { get; set; }

        [JsonProperty("latitude")]
        public string Latitude { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("outcode")]
        public string Outcode { get; set; }

        [JsonProperty("admin_district")]
        public string AdminDistrict { get; set; }

        [JsonProperty("admin_county")]
        public string AdminCounty { get; set; }

        [JsonProperty("codes")]
        public LocationCodesDto Codes { get; set; }
    }
}
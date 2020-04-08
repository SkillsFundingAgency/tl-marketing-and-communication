﻿using Newtonsoft.Json;

namespace sfa.Tl.Marketing.Communication.Models.Dto
{
    public class LocationCodesDto
    {
        [JsonProperty("admin_district")]
        public string AdminDistrict { get; set; }

        [JsonProperty("admin_county")]
        public string AdminCounty { get; set; }

        [JsonProperty("admin_ward")]
        public string AdminWard { get; set; }

        [JsonProperty("parish")]
        public string Parish { get; set; }

        [JsonProperty("parliamentary_constituency")]
        public string ParliamentaryConstituency { get; set; }

        [JsonProperty("ccg")]
        public string Ccg { get; set; }

        [JsonProperty("ced")]
        public string Ced { get; set; }

        [JsonProperty("nuts")]
        public string Nuts { get; set; }
    }
}
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace sfa.Tl.Marketing.Communication.Models.Entities
{
    public class LocationEntity
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("postcode")]
        public string Postcode { get; set; }
        [JsonPropertyName("town")]
        public string Town { get; set; }
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }
        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
        [JsonPropertyName("deliveryYears")]
        public IList<DeliveryYearEntity> DeliveryYears { get; set; }
        [JsonPropertyName("website")]
        public string Website { get; set; }
    }
}

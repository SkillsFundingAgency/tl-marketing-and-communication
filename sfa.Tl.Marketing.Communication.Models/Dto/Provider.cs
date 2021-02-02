using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace sfa.Tl.Marketing.Communication.Models.Dto
{
    public class Provider
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        public long UkPrn { get; set; }
        public string Name { get; set; }
        public IList<Location> Locations { get; set; }
    }
}
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace sfa.Tl.Marketing.Communication.Models.Dto
{
    public class Provider
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Location> Locations { get; set; }
    }
}
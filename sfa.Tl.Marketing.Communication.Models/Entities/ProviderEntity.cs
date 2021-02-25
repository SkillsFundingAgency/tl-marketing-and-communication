using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Azure.Cosmos.Table;

namespace sfa.Tl.Marketing.Communication.Models.Entities
{
    public class ProviderEntity : TableEntity
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("ukprn")]
        public long UkPrn { get; set; }

        public IList<LocationEntity> Locations { get; set; }
    }
}

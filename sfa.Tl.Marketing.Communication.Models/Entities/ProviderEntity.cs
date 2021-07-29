using System.Text.Json.Serialization;
using Microsoft.Azure.Cosmos.Table;

namespace sfa.Tl.Marketing.Communication.Models.Entities
{
    public class ProviderEntity : TableEntity
    {
        [JsonPropertyName("name")]
        public string Name { get; init; }

        [JsonPropertyName("ukprn")]
        public long UkPrn { get; init; }
    }
}

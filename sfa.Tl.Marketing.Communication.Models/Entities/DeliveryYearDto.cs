using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace sfa.Tl.Marketing.Communication.Models.Entities
{
    public class DeliveryYearEntity
    {
        [JsonPropertyName("year")]
        public short Year { get; init; }

        [JsonPropertyName("qualification")]
        public IList<int> Qualifications { get; init; }
    }
}

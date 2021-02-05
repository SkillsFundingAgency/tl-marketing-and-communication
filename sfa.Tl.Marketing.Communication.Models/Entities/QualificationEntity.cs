using System.Text.Json.Serialization;

namespace sfa.Tl.Marketing.Communication.Models.Entities
{
    public class QualificationEntity : Entity<int>
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}

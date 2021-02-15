using System.Text.Json.Serialization;

namespace sfa.Tl.Marketing.Communication.Models.Entities
{
    public class QualificationEntity : Entity<int>
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}

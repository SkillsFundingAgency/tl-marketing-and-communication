using System;
using System.Text.Json.Serialization;

namespace sfa.Tl.Marketing.Communication.Data.Entities
{
    public class QualificationEntity : Entity<int>
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        public QualificationEntity()
        {
            //TODO: Add a DateTimeService
            CreatedOn = DateTime.UtcNow;
        }
    }
}

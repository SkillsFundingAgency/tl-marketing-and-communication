using System;
using System.Text.Json.Serialization;

namespace sfa.Tl.Marketing.Communication.Data.Entities
{
    public class ProviderEntity : Entity<int>
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        //TODO: Fill in the rest of the properties
        public ProviderEntity()
        {
            //TODO: Add a DateTimeService
            CreatedOn = DateTime.UtcNow;
        }
    }
}

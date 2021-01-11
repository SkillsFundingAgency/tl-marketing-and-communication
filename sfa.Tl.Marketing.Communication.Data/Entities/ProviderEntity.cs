using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.Azure.Cosmos.Table;

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

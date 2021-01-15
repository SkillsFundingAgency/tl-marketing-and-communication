using System;
using System.Text.Json.Serialization;
using Microsoft.Azure.Cosmos.Table;

namespace sfa.Tl.Marketing.Communication.Models.Entities
{
    public class Entity<TKey> : TableEntity
    {
        [JsonPropertyName("id")]
        public TKey Id { get; set; }

        [JsonPropertyName("createdOn")]
        public DateTime CreatedOn { get; set; }

        protected Entity()
        {
            //TODO: Add a DateTimeService
            CreatedOn = DateTime.UtcNow;
        }
    }
}

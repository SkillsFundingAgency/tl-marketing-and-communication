using System;
using System.Text.Json.Serialization;
using Microsoft.Azure.Cosmos.Table;

namespace sfa.Tl.Marketing.Communication.Application.Entities
{
    public class Entity<TKey> : TableEntity
    {
        [JsonPropertyName("id")]
        public TKey Id { get; set; }

        //[JsonPropertyName("object")]
        //public string Object { get; set; }

        [JsonPropertyName("createdOn")]
        public DateTime CreatedOn { get; set; }

        protected Entity()
        {
            //TODO: Add a DateTimeService
            CreatedOn = DateTime.UtcNow;
        }
    }
}

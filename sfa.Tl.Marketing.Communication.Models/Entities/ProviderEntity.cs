using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Azure.Cosmos.Table;

namespace sfa.Tl.Marketing.Communication.Models.Entities
{
    public class ProviderEntity : Entity<long>
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("ukprn")]
        public long UkPrn { get; set; }

        //This property is serialized to json in the cloud table
        public IList<LocationEntity> Locations { get; set; }

        public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            var results = base.WriteEntity(operationContext);
            results.Add("Locations", new EntityProperty(JsonSerializer.Serialize(Locations)));
            return results;
        }

        public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            base.ReadEntity(properties, operationContext);

            var locationsProperty = properties.FirstOrDefault(p => p.Key == "Locations");

            if (!locationsProperty.Equals(default(KeyValuePair<string, EntityProperty>))
                && locationsProperty.Value != null)
            {
                Locations = JsonSerializer.Deserialize<IList<LocationEntity>>
                        (locationsProperty.Value.ToString());
            }
            else
            {
                Locations = new List<LocationEntity>();
            }
        }
    }
}

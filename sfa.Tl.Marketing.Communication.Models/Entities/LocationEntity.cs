using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Azure.Cosmos.Table;

namespace sfa.Tl.Marketing.Communication.Models.Entities
{
    public class LocationEntity : TableEntity
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("postcode")]
        public string Postcode { get; set; }
        [JsonPropertyName("town")]
        public string Town { get; set; }
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }
        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
        [JsonPropertyName("website")]
        public string Website { get; set; }

        //This property is serialized to json WriteEntity/ReadEntity below
        public IList<DeliveryYearEntity> DeliveryYears { get; set; }
        
        public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            var results = base.WriteEntity(operationContext);
            var serializedDeliveryYears = JsonSerializer.Serialize(DeliveryYears);
            results.Add("DeliveryYears", new EntityProperty(serializedDeliveryYears));
            return results;
        }

        public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            base.ReadEntity(properties, operationContext);

            var deliveryYearsProperty = properties
                .FirstOrDefault(p => p.Key == "DeliveryYears");

            DeliveryYears = !deliveryYearsProperty.Equals(default(KeyValuePair<string, EntityProperty>))
                            && deliveryYearsProperty.Value != null
                ? JsonSerializer.Deserialize<IList<DeliveryYearEntity>>
                    (deliveryYearsProperty.Value.ToString())
                : new List<DeliveryYearEntity>();
        }
    }
}

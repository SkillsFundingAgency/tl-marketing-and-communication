using System.Collections.Generic;
using System.Diagnostics;
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

        //[JsonPropertyName("deliveryYears")]
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

            if (!deliveryYearsProperty.Equals(default(KeyValuePair<string, EntityProperty>))
                && deliveryYearsProperty.Value != null)
            {
                DeliveryYears = JsonSerializer.Deserialize<IList<DeliveryYearEntity>>
                    (deliveryYearsProperty.Value.ToString());
            }
            else
            {
                DeliveryYears = new List<DeliveryYearEntity>();
            }
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Azure.Cosmos.Table;

namespace sfa.Tl.Marketing.Communication.Models.Entities;

public class LocationEntity : TableEntity
{
    [JsonPropertyName("name")]
    public string Name { get; init; }
    [JsonPropertyName("postcode")]
    public string Postcode { get; init; }
    [JsonPropertyName("town")]
    public string Town { get; init; }
    [JsonPropertyName("latitude")]
    public double Latitude { get; init; }
    [JsonPropertyName("longitude")]
    public double Longitude { get; init; }
    [JsonPropertyName("website")]
    public string Website { get; init; }

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
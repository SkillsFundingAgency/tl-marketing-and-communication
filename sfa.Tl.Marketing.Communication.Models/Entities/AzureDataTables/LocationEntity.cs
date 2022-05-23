using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure;
using Azure.Data.Tables;

namespace sfa.Tl.Marketing.Communication.Models.Entities.AzureDataTables;

public class LocationEntity : ITableEntity,
    IConvertibleEntity<LocationEntity, Entities.LocationEntity>
{
    [JsonPropertyName("name")] public string Name { get; init; }
    [JsonPropertyName("postcode")] public string Postcode { get; init; }
    [JsonPropertyName("town")] public string Town { get; init; }
    [JsonPropertyName("latitude")] public double Latitude { get; init; }
    [JsonPropertyName("longitude")] public double Longitude { get; init; }
    [JsonPropertyName("website")] public string Website { get; init; }

    [JsonPropertyName("DeliveryYears")]
    //public IList<DeliveryYearEntity> DeliveryYears { get; set; }
    public string DeliveryYears { get; set; }

    /*
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
    */

    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public Entities.LocationEntity Convert()
    {
        return new Entities.LocationEntity
        {
            Name = Name,
            Postcode = Postcode,
            Town = Town,
            Latitude = Latitude,
            Longitude = Longitude,
            Website = Website,
            DeliveryYears =
                !string.IsNullOrEmpty(DeliveryYears)
                    ? JsonSerializer.Deserialize<IList<DeliveryYearEntity>>
                        (DeliveryYears)
                    : new List<DeliveryYearEntity>(),
            PartitionKey = PartitionKey,
            RowKey = RowKey,
            Timestamp = Timestamp ?? default,
            ETag = ETag.ToString()
        };
    }
}
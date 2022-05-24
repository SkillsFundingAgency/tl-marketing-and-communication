using System;
using System.Text.Json.Serialization;
using Azure;
using Azure.Data.Tables;

namespace sfa.Tl.Marketing.Communication.Models.Entities.AzureDataTables;

public class LocationEntity : ITableEntity
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
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}
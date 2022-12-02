using System;
using System.Text.Json.Serialization;
using Azure;
using Azure.Data.Tables;

namespace sfa.Tl.Marketing.Communication.Models.Entities;

public class TownEntity : ITableEntity
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("cty")]
    public string County { get; init; }

    [JsonPropertyName("la")]
    public string LocalAuthority { get; init; }

    [JsonPropertyName("lat")]
    public decimal Latitude { get; init; }

    [JsonPropertyName("lon")]
    public decimal Longitude { get; init; }

    [JsonPropertyName("search")]
    public string SearchString { get; init; }

    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}
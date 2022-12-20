using System;
using System.Text.Json.Serialization;
using Azure;
using Azure.Data.Tables;
// ReSharper disable StringLiteralTypo

namespace sfa.Tl.Marketing.Communication.Models.Entities;

public class TownEntity : ITableEntity
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("county")]
    public string County { get; init; }

    [JsonPropertyName("localauthority")]
    public string LocalAuthority { get; init; }

    [JsonPropertyName("latitude")]
    public double Latitude { get; init; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; init; }

    [JsonPropertyName("search")]
    public string SearchString { get; init; }

    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}
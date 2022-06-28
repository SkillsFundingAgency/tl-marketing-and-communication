using System;
using System.Text.Json.Serialization;
using Azure;
using Azure.Data.Tables;

namespace sfa.Tl.Marketing.Communication.Models.Entities;

public class ProviderEntity : ITableEntity
{
    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("ukprn")]
    public long UkPrn { get; init; }

    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}
using System;
using System.Text.Json.Serialization;
using Azure;
using Azure.Data.Tables;

namespace sfa.Tl.Marketing.Communication.Models.Entities.AzureDataTables;

public class ProviderEntity : ITableEntity,
    IConvertibleEntity<ProviderEntity, Entities.ProviderEntity>
{
    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("ukprn")]
    public long UkPrn { get; init; }

    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public Entities.ProviderEntity Convert()
    {
        return new Entities.ProviderEntity
        {
            Name = Name,
            UkPrn = UkPrn,
            PartitionKey = PartitionKey,
            RowKey = RowKey,
            Timestamp = Timestamp ?? default,
            ETag = ETag.ToString()
        };
    }
}
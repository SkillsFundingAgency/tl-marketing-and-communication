using System;
using System.Text.Json.Serialization;
using Azure;
using Azure.Data.Tables;

namespace sfa.Tl.Marketing.Communication.Models.Entities.AzureDataTables;

public class QualificationEntity : ITableEntity,
    IConvertibleEntity<QualificationEntity, Entities.QualificationEntity>
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("route")]
    public string Route { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; }

    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public Entities.QualificationEntity Convert()
    {
        return new Entities.QualificationEntity
        {
            Id = this.Id,
            Name = this.Name,
            Route = this.Route,
            PartitionKey = this.PartitionKey,
            RowKey = this.RowKey,
            Timestamp = this.Timestamp ?? default,
            ETag = this.ETag.ToString()
        };
    }
}
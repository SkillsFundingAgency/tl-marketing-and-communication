using System.Text.Json.Serialization;
using Microsoft.Azure.Cosmos.Table;

namespace sfa.Tl.Marketing.Communication.Models.Entities;

public class QualificationEntity : TableEntity
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("route")]
    public string Route { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; }
}
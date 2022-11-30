using System.Diagnostics;
using System.Text.Json.Serialization;

namespace sfa.Tl.Marketing.Communication.Models.Dto;

[DebuggerDisplay(" {" + nameof(Name) + ", nq}" +
                 " ({" + nameof(County) + ", nq})")]
public class Town
{
    public int Id { get; init; }
    public string Name { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string County { get; init; }
    [JsonPropertyName("la")]
    public string LocalAuthority { get; init; }
    [JsonIgnore]
    public decimal Latitude { get; init; }
    [JsonIgnore]
    public decimal Longitude { get; init; }
}
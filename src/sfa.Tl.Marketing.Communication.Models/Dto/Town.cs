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
    public double Latitude { get; init; }
    [JsonIgnore]
    public double Longitude { get; init; }
    [JsonIgnore]
    public string SearchString { get; init; }

    [JsonIgnore]
    public string DisplayName =>
        !string.IsNullOrEmpty(County) 
            ? $"{Name}, {County}" 
            : !string.IsNullOrEmpty(LocalAuthority) 
                ? $"{Name}, {LocalAuthority}" 
                : Name;
}
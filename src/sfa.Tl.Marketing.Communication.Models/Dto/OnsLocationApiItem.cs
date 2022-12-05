using System;
using System.Diagnostics;
using sfa.Tl.Marketing.Communication.Models.Enums;

namespace sfa.Tl.Marketing.Communication.Models.Dto;

[DebuggerDisplay(" {" + nameof(LocationName) + "}" +
                 " ({" + nameof(CountyName) + ", nq})")]
public class OnsLocationApiItem
{
    public int Id { get; init; }
    public string LocationName { get; init; }
    public string CountyName { get; init; }
    public string Country { get; init; }
    public string LocalAuthorityName { get; init; }

    public string LocalAuthorityDistrictDescription { get; init; }
    public LocalAuthorityDistrict LocalAuthorityDistrict =>
        Enum.TryParse<LocalAuthorityDistrict>(LocalAuthorityDistrictDescription,
            out var localAuthorityDistrict)
            ? localAuthorityDistrict
            : default;

    public string LocationAuthorityDistrict { get; init; }

    public string PlaceNameDescription { get; init; }
    public PlaceNameDescription PlaceName => 
        Enum.TryParse<PlaceNameDescription>(PlaceNameDescription,
            out var placeName)
            ? placeName
            : default;

    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public int PopulationCount { get; init; }
}
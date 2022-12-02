﻿using System.Diagnostics;
using sfa.Tl.Marketing.Communication.Models.Enums;

namespace sfa.Tl.Marketing.Communication.Models.Dto;

[DebuggerDisplay(" {" + nameof(LocationName) + "}" +
                 " ({" + nameof(CountyName) + ", nq})")]
public class OnsLocationApiItem
{
    public int Id { get; init; }
    public string LocationName { get; init; }
    public string CountyName { get; init; }
    public string LocalAuthorityName { get; init; }
    public LocalAuthorityDistrict LocalAuthorityDistrict { get; init; }
    public string LocalAuthorityDistrictDescription { get; init; }
    public string LocationAuthorityDistrict { get; init; }
    public PlaceNameDescription PlaceName{ get; init; }
    public string PlaceNameDescription { get; init; }
    public decimal Latitude { get; init; }
    public decimal Longitude { get; init; }
}
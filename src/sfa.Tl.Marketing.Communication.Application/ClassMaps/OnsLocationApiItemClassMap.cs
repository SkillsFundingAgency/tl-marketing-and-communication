using CsvHelper.Configuration;
using sfa.Tl.Marketing.Communication.Models.Dto;

// ReSharper disable StringLiteralTypo

namespace sfa.Tl.Marketing.Communication.Application.ClassMaps;

public sealed class OnsLocationApiItemClassMap : ClassMap<OnsLocationApiItem>
{
    public OnsLocationApiItemClassMap()
    {
        Map(m => m.Id).Name("placeid");
        Map(m => m.LocationName).Name("place15nm");
        Map(m => m.CountyName).Name("cty15nm");
        Map(m => m.Country).Name("ctry15nm");
        Map(m => m.LocalAuthorityName).Name("ctyltnm");
        Map(m => m.LocalAuthorityDistrictDescription).Name("laddescnm");
        Map(m => m.LocationAuthorityDistrict).Name("lad15nm");
        Map(m => m.PlaceNameDescription).Name("descnm");
        Map(m => m.Latitude).Name("lat");
        Map(m => m.Longitude).Name("long");
        Map(m => m.PopulationCount).Name("popcnt");
    }
}

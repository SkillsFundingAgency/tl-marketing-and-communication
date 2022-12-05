using System.Collections.Generic;
using sfa.Tl.Marketing.Communication.Application.Extensions;
using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.Functions.UnitTests.Builders;

public class TownBuilder
{
    public IList<Town> BuildList() => new List<Town>
    {
        new()
        {
            Id = 1,
            Name = "Coventry",
            County = "West Midlands",
            LocalAuthority = "West Midlands",
            Latitude = 52.41695,
            Longitude = -1.50721
        },
        new()
        {
            Id = 2,
            Name = "Oxford",
            County = "Oxfordshire",
            LocalAuthority = "Oxfordshire",
            Latitude = 51.740811,
            Longitude = -1.217524
        }
    };

    public string BuildJson() =>
        $"{GetType().Namespace}.Data.TownData.json"
            .ReadManifestResourceStreamAsString();
}
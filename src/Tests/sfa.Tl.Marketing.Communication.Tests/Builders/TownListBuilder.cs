﻿using System.Collections.Generic;
using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.UnitTests.Builders;

internal class TownListBuilder
{
    public IEnumerable<Town> Build() =>
        new List<Town>
        {
            new()
            {
                Id = 1,
                Name = "Coventry",
                County = "West Midlands",
                LocalAuthority = "West Midlands",
                Latitude = 52.41695M,
                Longitude = -1.50721M
            },
            new()
            {
                Id = 2,
                Name = "Oxford",
                County = "Oxfordshire",
                LocalAuthority = "Oxfordshire",
                Latitude = 51.740811M,
                Longitude = -1.217524M
            }
        };
}
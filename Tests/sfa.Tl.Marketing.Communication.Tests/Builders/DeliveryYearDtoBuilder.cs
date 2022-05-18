using System.Collections.Generic;
using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.UnitTests.Builders;

public class DeliveryYearDtoBuilder
{
    public DeliveryYearDto Build() => new()
    {
        Year = 2020,
        Qualifications = new List<int>
        {
            1,
        }
    };
}
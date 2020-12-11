using System.Collections.Generic;
using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.UnitTests.Builders
{
    public class DeliveryYearBuilder
    {
        public DeliveryYear Build() => new DeliveryYear
        {
            Year = 2020,
            Qualifications = new List<Qualification>
            {
                new Qualification
                {
                    Id = 1,
                    Name = "Test Qualification"
                }
            }
        };
    }
}

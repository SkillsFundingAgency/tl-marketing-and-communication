using System.Collections.Generic;
using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.UnitTests.Builders
{
    public class ProviderBuilder
    {
        public Provider Build() => new Provider
        {
            Id = 1,
            Name = "Test Provider",
            Locations = new List<Location>
            {
                new Location
                {
                    Postcode = "CV1 2WT",
                    Town = "Coventry",
                    Latitude = 52.400997,
                    Longitude = -1.508122,
                    DeliveryYears = new List<DeliveryYearDto>
                    {
                        new DeliveryYearDto
                        {
                            Year = 2021,
                            Qualifications = new List<int>
                            {
                                1
                            }
                        }
                    },
                    Website = "https://test.provider.co.uk"
                }
            }
        };
    }
}

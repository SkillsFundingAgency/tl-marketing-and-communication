using System.Collections.Generic;
using sfa.Tl.Marketing.Communication.Application.Extensions;
using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.Functions.UnitTests.Builders
{
    public class ProviderBuilder
    {
        public IList<Provider> BuildList() => new List<Provider>
        {
            new Provider
            {
                Id = 1,
                UkPrn = 12345678,
                Name = "Test Provider 1",
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
            }
        };

        public string BuildJson() =>
            $"{GetType().Namespace}.Data.ProviderData.json"
                .ReadManifestResourceStreamAsString();
    }
}

using System.Collections.Generic;
using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.UnitTests.Builders
{
    public class ProviderLocationBuilder
    {
        public ProviderLocation Build() => new ProviderLocation
        {
            ProviderName = "Test Provider",
            Name = "Test Location",
            Postcode = "CV1 2WT",
            Town = "Coventry",
            Latitude = 52.400997,
            Longitude = -1.508122,
            DistanceInMiles = 9.5,
            DeliveryYears = new List<DeliveryYear>(),
            Website = "https://test.provider.co.uk"
        };
    }
}

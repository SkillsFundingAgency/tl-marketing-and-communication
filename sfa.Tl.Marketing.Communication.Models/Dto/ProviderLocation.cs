using System.Collections.Generic;
using System.Diagnostics;

namespace sfa.Tl.Marketing.Communication.Models.Dto
{
    [DebuggerDisplay("{" + nameof(ProviderName) + "}" +
                     " {" + nameof(Postcode) + ", nq}")]
    public class ProviderLocation
    {
        public string ProviderName { get; init; }
        public string Name { get; init; }
        public string Postcode { get; init; }
        public string Town { get; init; }
        public double Latitude { get; init; }
        public double Longitude { get; init; }
        public double DistanceInMiles { get; set; }
        public IEnumerable<DeliveryYear> DeliveryYears { get; init; }
        public string Website { get; init; }
        public string JourneyUrl { get; set; }
    }
}

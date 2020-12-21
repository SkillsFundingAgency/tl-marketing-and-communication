using System.Collections.Generic;

namespace sfa.Tl.Marketing.Communication.Models.Dto
{
    public class ProviderLocation
    {
        public string ProviderName { get; set; }
        public string Name { get; set; }
        public string Postcode { get; set; }
        public string Town { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double DistanceInMiles { get; set; }
        public IEnumerable<DeliveryYear> DeliveryYears { get; set; }
        public string Website { get; set; }
    }
}

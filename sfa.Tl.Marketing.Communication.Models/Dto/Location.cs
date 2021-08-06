using System.Collections.Generic;

namespace sfa.Tl.Marketing.Communication.Models.Dto
{
    public class Location
    {
        public string Name { get; set; }
        public string Postcode { get; init; }
        public string Town { get; init; }
        public double Latitude { get; init; }
        public double Longitude { get; init; }
        public IList<DeliveryYearDto> DeliveryYears { get; init; }
        public string Website { get; init; }
    }
}

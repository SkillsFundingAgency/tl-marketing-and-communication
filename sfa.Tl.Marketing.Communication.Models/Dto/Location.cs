using System.Collections.Generic;

namespace sfa.Tl.Marketing.Communication.Models.Dto
{
    public class Location
    {
        public string Name { get; set; }
        public string Postcode { get; set; }
        public string Town { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public List<DeliveryYearDto> DeliveryYears { get; set; }
        public string Website { get; set; }
    }
}

using System.Collections.Generic;

namespace sfa.Tl.Marketing.Communication.DataLoad.Write
{
    public class LocationWriteData
    {
        public string Name { get; set; }
        public string Postcode { get; set; }
        public string Town { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public List<DeliveryYearWriteData> DeliveryYears { get; set; }
        public string Website { get; set; }
    }
}

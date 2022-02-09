using System.Collections.Generic;
using System.Diagnostics;

namespace sfa.Tl.Marketing.Communication.Models.Dto
{
    [DebuggerDisplay("{" + nameof(Postcode) + "}" +
                     " {" + nameof(Name) + ", nq}")]
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

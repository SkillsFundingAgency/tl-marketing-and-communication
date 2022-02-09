
using System.Diagnostics;

namespace sfa.Tl.Marketing.Communication.Models.Dto
{
    [DebuggerDisplay(" {" + nameof(Postcode) + ", nq}" +
                     " ({" + nameof(Latitude) + "}, {" + nameof(Longitude) + "})")]
    public class PostcodeLocation
    {
        public string Postcode { get; init; }
        public double? Longitude { get; init; }
        public double? Latitude { get; init; }
   }
}
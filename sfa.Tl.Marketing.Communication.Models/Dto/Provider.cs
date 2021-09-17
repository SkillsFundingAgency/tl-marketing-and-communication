using System.Collections.Generic;
using System.Diagnostics;

namespace sfa.Tl.Marketing.Communication.Models.Dto
{
    [DebuggerDisplay("UKPRN {" + nameof(UkPrn) + "}" +
                     " {" + nameof(Name) + ", nq}")]
    public class Provider
    {
        public long UkPrn { get; init; }
        public string Name { get; init; }
        public IList<Location> Locations { get; set; }
    }
}
using System.Collections.Generic;
using System.Diagnostics;

namespace sfa.Tl.Marketing.Communication.Models.Dto
{
    [DebuggerDisplay("{DebuggerDisplay(), nq}")]
    public class DeliveryYear
    {
        public short Year { get; init; }

        public bool IsAvailableNow { get; set; }

        public IList<Qualification> Qualifications { get; set; }
        
        private string DebuggerDisplay()
            => $"{Year} " +
               $"{(IsAvailableNow ? "(Available now) " : "")}" +
               $"{(Qualifications != null ? Qualifications.Count : "null")} Qualifications";
    }
}

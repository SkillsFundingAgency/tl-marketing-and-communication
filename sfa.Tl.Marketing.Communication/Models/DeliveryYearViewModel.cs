using System.Collections.Generic;
using System.Diagnostics;

namespace sfa.Tl.Marketing.Communication.Models;

[DebuggerDisplay("{DebuggerDisplay(), nq}")]
public class DeliveryYearViewModel
{
    public short Year { get; set; }
        
    public bool IsAvailableNow { get; set; }

    public IList<QualificationViewModel> Qualifications { get; set; }

    private string DebuggerDisplay()
        => $"{Year} " +
           $"{(IsAvailableNow ? "(Available now) " : "")}" +
           $"{(Qualifications != null ? Qualifications.Count : "null")} Qualifications";
}
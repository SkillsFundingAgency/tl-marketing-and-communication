﻿using System.Collections.Generic;
using System.Diagnostics;

namespace sfa.Tl.Marketing.Communication.Models.Dto;

[DebuggerDisplay("{DebuggerDisplay(), nq}")]
public class DeliveryYear
{
    public short Year { get; init; }

    public IList<Qualification> Qualifications { get; init; }
        
    private string DebuggerDisplay()
        => $"{Year} " +
           $"{(Qualifications != null ? Qualifications.Count : "null")} Qualifications";
}
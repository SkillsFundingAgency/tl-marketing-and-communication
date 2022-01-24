using System.Diagnostics;

namespace sfa.Tl.Marketing.Communication.Models;

[DebuggerDisplay(" {" + nameof(Id) + "}" +
                 " {" + nameof(Name) + ", nq}")]
public class QualificationViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
}
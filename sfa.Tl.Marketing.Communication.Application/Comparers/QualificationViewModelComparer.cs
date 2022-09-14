using System.Collections.Generic;
using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.Application.Comparers;

public class QualificationComparer : IEqualityComparer<Qualification>
{
    public bool Equals(Qualification q1, Qualification q2)
    {
        if (ReferenceEquals(q1, null)) return false;
        if (ReferenceEquals(q2, null)) return false;
        if (ReferenceEquals(q1, q2)) return true;
        if (q1.GetType() != q2.GetType()) return false;

        return q1.Id == q2.Id;
    }

    public int GetHashCode(Qualification q)
    {
        return q.Id;
    }
}

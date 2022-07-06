using System.Collections.Generic;
using sfa.Tl.Marketing.Communication.Models;

namespace sfa.Tl.Marketing.Communication.Comparers;

public class QualificationViewModelComparer : IEqualityComparer<QualificationViewModel>
{
    public bool Equals(QualificationViewModel q1, QualificationViewModel q2)
    {
        if (ReferenceEquals(q1, null)) return false;
        if (ReferenceEquals(q2, null)) return false;
        if (ReferenceEquals(q1, q2)) return true;
        if (q1.GetType() != q2.GetType()) return false;

        return q1.Id == q2.Id;
    }

    public int GetHashCode(QualificationViewModel q)
    {
        return q.Id;
    }
}

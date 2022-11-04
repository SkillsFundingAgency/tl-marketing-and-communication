using System;
using System.Diagnostics;

namespace sfa.Tl.Marketing.Communication.Models.Dto;

[DebuggerDisplay(" {" + nameof(Id) + "}" +
                 " {" + nameof(Name) + ", nq}")]
public class Qualification : IEquatable<Qualification>
{
    public int Id { get; init; }
    public string Route { get; init; }
    public string Name { get; init; }

    public bool Equals(Qualification other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id && Route == other.Route && Name == other.Name;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Qualification)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Route, Name);
    }
}
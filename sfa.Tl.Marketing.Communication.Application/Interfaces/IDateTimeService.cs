using System;

// ReSharper disable UnusedMember.Global

namespace sfa.Tl.Marketing.Communication.Application.Interfaces;

public interface IDateTimeService
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
    DateTime Today { get; }
}
using System;

// ReSharper disable ClassNeverInstantiated.Global

namespace sfa.Tl.Marketing.Communication.Functions.Timers
{
    public class ScheduleStatus
    {
        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
using System.Collections.Generic;

namespace sfa.Tl.Marketing.Communication.Models.Dto
{
    public class Provider
    {
        public long UkPrn { get; init; }
        public string Name { get; init; }
        public IList<Location> Locations { get; set; }
    }
}
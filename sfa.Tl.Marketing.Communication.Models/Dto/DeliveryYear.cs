using System.Collections.Generic;

namespace sfa.Tl.Marketing.Communication.Models.Dto
{
    public class DeliveryYear
    {
        public short Year { get; init; }

        public IEnumerable<Qualification> Qualifications { get; init; }
    }
}

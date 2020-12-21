using System.Collections.Generic;

namespace sfa.Tl.Marketing.Communication.Models.Dto
{
    public class DeliveryYear
    {
        public short Year { get; set; }

        public IEnumerable<Qualification> Qualifications { get; set; }
    }
}

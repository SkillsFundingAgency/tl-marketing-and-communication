using System.Collections.Generic;

namespace sfa.Tl.Marketing.Communication.Models.Dto
{
    public class DeliveryYearDto
    {
        public short Year { get; set; }

        public IList<int> Qualifications { get; set; }
    }
}

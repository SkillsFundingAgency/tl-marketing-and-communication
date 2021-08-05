using System.Collections.Generic;

namespace sfa.Tl.Marketing.Communication.Models.Dto
{
    public class DeliveryYearDto
    {
        public short Year { get; init; }

        public IList<int> Qualifications { get; init; }
    }
}

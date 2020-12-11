using System.Collections.Generic;

namespace sfa.Tl.Marketing.Communication.Models
{
    public class DeliveryYearViewModel
    {
        public short Year { get; set; }
        public IList<QualificationViewModel> Qualifications { get; set; }
    }
}

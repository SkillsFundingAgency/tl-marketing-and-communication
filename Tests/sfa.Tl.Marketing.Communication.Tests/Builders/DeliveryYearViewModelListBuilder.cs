using System.Collections.Generic;
using sfa.Tl.Marketing.Communication.Models;

namespace sfa.Tl.Marketing.Communication.UnitTests.Builders
{
    public class DeliveryYearViewModelListBuilder
    {
        public IList<DeliveryYearViewModel> Build() =>
            new List<DeliveryYearViewModel>
            {
                new()
                {
                    Year = 2021,
                    Qualifications = new List<QualificationViewModel>
                    {
                        new() { Id = 1, Name = "Test Qualification 1" }
                    }
                },
                new()
                {
                    Year = 2021,
                    Qualifications = new List<QualificationViewModel>
                    {
                        new() { Id = 2, Name = "Test Qualification 2" }
                    }
                }
            };
    }
}

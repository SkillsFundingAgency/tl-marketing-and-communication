using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.UnitTests.Builders
{
    public class QualificationBuilder
    {
        public Qualification Build() => new Qualification
        {
            Id = 1,
            Name = "Test Qualification"
        };
    }
}

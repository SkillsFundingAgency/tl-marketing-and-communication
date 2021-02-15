using System.Collections.Generic;
using sfa.Tl.Marketing.Communication.Application.Extensions;
using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.Functions.UnitTests.Builders
{
    public class QualificationBuilder
    {
        public IList<Qualification> BuildList() => new List<Qualification>
        {
            new()
            {
                Id = 1,
                Name = "Test Qualification 1"
            }
        };

        public string BuildJson() =>
            $"{GetType().Namespace}.Data.QualificationData.json"
                .ReadManifestResourceStreamAsString();
    }
}

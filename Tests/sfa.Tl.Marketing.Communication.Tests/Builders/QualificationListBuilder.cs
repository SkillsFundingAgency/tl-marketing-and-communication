using System;
using System.Collections.Generic;
using System.Text;
using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.UnitTests.Builders
{
    public class QualificationListBuilder
    {
        private readonly IList<Qualification> _qualifications = new List<Qualification>();

        public IList<Qualification> Build() =>
            _qualifications;

        public QualificationListBuilder Add(int numberOfQualifications = 1)
        {
            var start = _qualifications.Count;
            for (var i = 0; i < numberOfQualifications; i++)
            {
                var nextId = start + i + 1;
                _qualifications.Add(new Qualification
                {
                    Id = nextId,
                    Name = $"Test Qualification {nextId}"
                });
            }

            return this;
        }
    }
}

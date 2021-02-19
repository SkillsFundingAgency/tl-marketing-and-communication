using System.Collections.Generic;
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

        public QualificationListBuilder CreateKnownList()
        {
            _qualifications.Clear();
            _qualifications.Add(new Qualification
            {
                Id = 1,
                Name = "Qualification 1"
            });
            _qualifications.Add(new Qualification
            {
                Id = 37,
                Name = "Old Name for Digital Production, Design and Development"
            });
            _qualifications.Add(new Qualification
            {
                Id = 38,
                Name = "Education and Childcare"
            });
            _qualifications.Add(new Qualification
            {
                Id = 40,
                Name = "Digital Business Services"
            });
            _qualifications.Add(new Qualification
            {
                Id = 41,
                Name = "Building Services Engineering for Construction"
            });
            _qualifications.Add(new Qualification
            {
                Id = 42,
                Name = "Onsite Construction"
            });
            _qualifications.Add(new Qualification
            {
                Id = 43,
                Name = "Laboratory Science"
            });
            _qualifications.Add(new Qualification
            {
                Id = 44,
                Name = "Health"
            });
            _qualifications.Add(new Qualification
            {
                Id = 45,
                Name = "Healthcare Science"
            });
            _qualifications.Add(new Qualification
            {
                Id = 99,
                Name = "One to delete"
            });

            return this;
        }

    }
}

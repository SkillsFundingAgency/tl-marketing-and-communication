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

            /*
      "frameworkCode": 36, "name": "T Level Construction - Design, Surveying and Planning for Construction"
      "frameworkCode": 37, "name": "T Level Digital - Digital Production, Design and Development"
      "frameworkCode": 38, "name": "T Level Education - Education and Childcare"
      "frameworkCode": 39, "name": "T Level Digital - Digital Business Services"
      "frameworkCode": 40, "name": "T Level Digital - Digital Support Services"
      "frameworkCode": 41, "name": "T Level Health and Science - Health"
      "frameworkCode": 42, "name": "T Level Health and Science - Healthcare Science"
      "frameworkCode": 43, "name": "T Level Health and Science - Science"
      "frameworkCode": 44, "name": "T Level Construction - Onsite construction"
      "frameworkCode": 45, "name": "T Level Construction - Building services engineering for construction"
            */
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
                Id = 39,
                Name = "Digital Business Services"
            });
            _qualifications.Add(new Qualification
            {
                Id = 41,
                Name = "Health"
            });
            _qualifications.Add(new Qualification
            {
                Id = 42,
                Name = "Healthcare Science"
            });
            _qualifications.Add(new Qualification
            {
                Id = 43,
                Name = "Laboratory Science"
            });
            _qualifications.Add(new Qualification
            {
                Id = 44,
                Name = "Onsite construction"
            });
            _qualifications.Add(new Qualification
            {
                Id = 45,
                Name = "Building services engineering for construction"
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

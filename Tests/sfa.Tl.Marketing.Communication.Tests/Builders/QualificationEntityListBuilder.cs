using System.Collections.Generic;
using sfa.Tl.Marketing.Communication.Models.Entities;

namespace sfa.Tl.Marketing.Communication.UnitTests.Builders
{
    public class QualificationEntityListBuilder
    {
        private readonly IList<QualificationEntity> _qualificationEntities = new List<QualificationEntity>();

        public IList<QualificationEntity> Build() =>
            _qualificationEntities;

        public QualificationEntityListBuilder Add(int numberOfQualificationEntities = 1)
        {
            var start = _qualificationEntities.Count;
            for (var i = 0; i < numberOfQualificationEntities; i++)
            {
                var nextId = start + i + 1;
                _qualificationEntities.Add(new QualificationEntity
                {
                    Id = nextId,
                    Route = $"Route {nextId}",
                    Name = $"Test Qualification {nextId}"
                });
            }

            return this;
        }
    }
}

using System.Collections.Generic;
using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.UnitTests.Builders;

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
                Route = $"Route {nextId}",
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
            Route = "Test",
            Name = "Qualification 1"
        });
        _qualifications.Add(new Qualification
        {
            Id = 37,
            Route = "Digital",
            Name = "Old Name for Digital Production, Design and Development"
        });
        _qualifications.Add(new Qualification
        {
            Id = 38,
            Route = "Education",
            Name = "Education and Childcare"
        });
        _qualifications.Add(new Qualification
        {
            Id = 39,
            Route = "Digital",
            Name = "Digital Business Services"
        });
        _qualifications.Add(new Qualification
        {
            Id = 41,
            Route = "Health and Science",
            Name = "Health"
        });
        _qualifications.Add(new Qualification
        {
            Id = 42,
            Route = "Health and Science",
            Name = "Healthcare Science"
        });
        _qualifications.Add(new Qualification
        {
            Id = 43,
            Route = "Health and Science",
            Name = "Laboratory Science"
        });
        _qualifications.Add(new Qualification
        {
            Id = 44,
            Route = "Construction",
            Name = "Onsite Construction"
        });
        _qualifications.Add(new Qualification
        {
            Id = 45,
            Route = "Construction",
            Name = "Building services engineering for construction"
        });
        _qualifications.Add(new Qualification
        {
            Id = 99,
            Route = "None",
            Name = "One to delete"
        });

        return this;
    }

}
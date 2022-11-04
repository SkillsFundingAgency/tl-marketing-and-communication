using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using sfa.Tl.Marketing.Communication.Application.Extensions;
using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.UnitTests.Builders;

internal class TestQualificationsFromJsonBuilder
{
    public IList<Qualification> Build()
    {
        var json = $"{GetType().Namespace}.Data.test_qualifications.json"
            .ReadManifestResourceStreamAsString();

        return JsonDocument.Parse(json)
            .RootElement
            .GetProperty("qualifications")
            .EnumerateObject()
            .Select(q =>
                new Qualification
                {
                    Id = int.Parse(q.Name),
                    Name = q.Value.GetString()
                })
            //.OrderBy(q => q.Id)
            .ToList();
    }
}
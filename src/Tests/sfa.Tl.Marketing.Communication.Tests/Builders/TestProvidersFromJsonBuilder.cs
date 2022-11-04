using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using sfa.Tl.Marketing.Communication.Application.Extensions;
using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.UnitTests.Builders;

internal class TestProvidersFromJsonBuilder
{
    public IList<Provider> Build()
    {
        var json =
            $"{GetType().Namespace}.Data.test_providers.json"
                .ReadManifestResourceStreamAsString();

        return JsonDocument.Parse(json)
            .RootElement
            .GetProperty("providers")
            .EnumerateArray()
            .Select(p =>
                new Provider
                {
                    UkPrn = p.GetProperty("ukPrn").GetInt64(),
                    Name = p.GetProperty("name").GetString(),
                    Locations = p.GetProperty("locations")
                        .EnumerateArray()
                        .Select(l =>
                            new Location
                            {
                                Postcode = l.GetProperty("postcode").GetString(),
                                Name = l.GetProperty("name").GetString(),
                                Town = l.GetProperty("town").GetString(),
                                Latitude = l.GetProperty("latitude").GetDouble(),
                                Longitude = l.GetProperty("longitude").GetDouble(),
                                Website = l.GetProperty("website").GetString(),
                                DeliveryYears = l.TryGetProperty("deliveryYears", out var deliveryYears)
                                    ? deliveryYears.EnumerateArray()
                                        .Select(d =>
                                            new DeliveryYearDto
                                            {
                                                Year = d.GetProperty("year").GetInt16(),
                                                Qualifications = d.GetProperty("qualifications")
                                                    .EnumerateArray()
                                                    .Select(q => q.GetInt32())
                                                    .ToList()
                                            })
                                        .ToList()
                                    : new List<DeliveryYearDto>()
                            }).ToList()
                })
            .ToList();
    }

    public JsonDocument BuildJsonDocument()
    {
        var json =
            $"{GetType().Namespace}.Data.test_providers.json"
                .ReadManifestResourceStreamAsString();

        return JsonDocument.Parse(json);
    }
}
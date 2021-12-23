﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.Application.Extensions;

public static class TempProviderDataExtensions
{
    public static readonly IDictionary<long, Provider> ProviderData
        = LoadTempProviderData();

    private static IDictionary<long, Provider> LoadTempProviderData()
    {
        var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        var jsonFile = $"{assemblyName}.Data.ProviderData.json";

        return JsonDocument
            .Parse(jsonFile.ReadManifestResourceStreamAsString())
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
                                Name = JsonExtensions.SafeGetString(l, "name"),
                                Town = JsonExtensions.SafeGetString(l, "town"),
                                Latitude = JsonExtensions.SafeGetDouble(l, "latitude"),
                                Longitude = JsonExtensions.SafeGetDouble(l, "longitude"),
                                Website = JsonExtensions.SafeGetString(l, "website"),
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
            .ToDictionary(p => p.UkPrn);
    }

    public static IList<Provider> MergeTempProviders(
        this IList<Provider> providers, 
        bool shouldMerge = false)
    {
        if (!shouldMerge)
        {
            return providers;
        }

        foreach (var tempProvider in ProviderData)
        {
            //TODO: Create a dictionary of current providers to improve lookup
            if (providers.All(p => p.UkPrn != tempProvider.Key))
            {
                providers.Add(tempProvider.Value);
            }
        }

        return providers;
    }
}
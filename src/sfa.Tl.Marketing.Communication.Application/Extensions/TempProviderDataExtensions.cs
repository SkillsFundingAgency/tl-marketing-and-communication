using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.Application.Extensions;

public static class TempProviderDataExtensions
{
    public const string TempDataBlobContainerName = "provider-data";
    public const string TempDataFileName = "additional-providers.json";

    public static readonly IDictionary<long, Provider> TempProviderData
        = LoadTempProviderData();

    private static IDictionary<long, Provider> LoadTempProviderData()
    {
        var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        var jsonFile = $"{assemblyName}.Assets.ProviderData.json";

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
                                Name = l.SafeGetString("name"),
                                Town = l.SafeGetString("town"),
                                Latitude = l.SafeGetDouble("latitude"),
                                Longitude = l.SafeGetDouble("longitude"),
                                Website = l.SafeGetString("website"),
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

        var providerDictionary = providers
            .ToDictionary(p => p.UkPrn);

        foreach (var (key, tempProvider) in TempProviderData)
        {
            if (!providerDictionary.ContainsKey(key))
            {
                providerDictionary.Add(key, tempProvider);
            }
        }

        return providerDictionary
            .Values
            .ToList();
    }
}
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.Application.Extensions;

public static class TempProviderDataExtensions
{
    public const string TempDataBlobContainerName = "provider-data";
    public const string TempDataFileName = "additional-providers.json";

    public static IDictionary<long, Provider> TempProviderData
    {
        get;
        private set; 
    } = new Dictionary<long, Provider>();

    private static IDictionary<long, Provider> LoadTempProviderData(JsonDocument jsonDocument)
    {
        return jsonDocument
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
        Stream stream,
        bool shouldMerge = false)
    {
        if (stream != null)
        {
            var jsonDocument = JsonDocument.Parse(stream);
            var json = jsonDocument.PrettifyJsonDocument();

            return MergeTempProviders(providers, jsonDocument, shouldMerge);
        }

        return providers;
    }

    public static IList<Provider> MergeTempProviders(
        this IList<Provider> providers,
        JsonDocument jsonDocument,
        bool shouldMerge = false)
    {
        if (!shouldMerge || jsonDocument is null)
        {
            return providers;
        }

        var tempProviderData = LoadTempProviderData(jsonDocument);
        TempProviderData.Clear();
        foreach (var (key, tempProvider) in tempProviderData)
        {
            TempProviderData.Add(key, tempProvider);
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
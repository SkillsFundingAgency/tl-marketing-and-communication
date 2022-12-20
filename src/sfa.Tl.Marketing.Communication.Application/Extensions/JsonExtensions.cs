using System.IO;
using System.Text;
using System.Text.Json;

namespace sfa.Tl.Marketing.Communication.Application.Extensions;

public static class JsonExtensions
{
    public static string PrettifyJsonString(this string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return string.Empty;
        }

        var jsonDocument = JsonDocument.Parse(json);

        var options = new JsonWriterOptions
        {
            Indented = true
        };

        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream, options))
        {
            jsonDocument.WriteTo(writer);
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }

    public static string PrettifyJsonDocument(this JsonDocument jsonDocument)
    {
        if (jsonDocument is null)
        {
            return string.Empty;
        }

        var options = new JsonWriterOptions
        {
            Indented = true
        };

        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream, options))
        {
            jsonDocument.WriteTo(writer);
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }

    public static int SafeGetInt32(this JsonElement element, string propertyName)
    {
        return element.ValueKind != JsonValueKind.Undefined
               && element.TryGetProperty(propertyName, out var property)
               && property.ValueKind == JsonValueKind.Number
               && property.TryGetInt32(out var val)
            ? val
            : default;
    }

    public static long SafeGetInt64(this JsonElement element, string propertyName)
    {
        return element.ValueKind != JsonValueKind.Undefined
               && element.TryGetProperty(propertyName, out var property)
               && property.ValueKind == JsonValueKind.Number
               && property.TryGetInt64(out var val)
            ? val
            : default;
    }
    
    public static decimal SafeGetDecimal(this JsonElement element,
        string propertyName,
        decimal defaultValue = default)
    {
        return element.ValueKind != JsonValueKind.Undefined
               && element.TryGetProperty(propertyName, out var property)
               && property.ValueKind == JsonValueKind.Number
               && property.TryGetDecimal(out var val)
            ? val
            : defaultValue;
    }

    public static double SafeGetDouble(this JsonElement element, string propertyName, double defaultValue = default)
    {
        return element.ValueKind != JsonValueKind.Undefined
               && element.TryGetProperty(propertyName, out var property)
               && property.ValueKind == JsonValueKind.Number
               && property.TryGetDouble(out var val)
            ? val
            : defaultValue;
    }

    public static string SafeGetString(this JsonElement element, string propertyName, int maxLength = -1)
    {
        var result = element.ValueKind != JsonValueKind.Undefined
                     && element.TryGetProperty(propertyName, out var property)
                     && property.ValueKind == JsonValueKind.String
            ? property.GetString()
            : default;

        return result is not null && maxLength > 0 && result.Length > maxLength
            ? result[..maxLength].Trim()
            : result;
    }
}
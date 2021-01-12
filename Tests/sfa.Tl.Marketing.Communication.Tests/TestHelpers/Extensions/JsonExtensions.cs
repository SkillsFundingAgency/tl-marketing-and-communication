using System.IO;
using System.Text;
using System.Text.Json;

namespace sfa.Tl.Marketing.Communication.UnitTests.TestHelpers.Extensions
{
    public static class JsonExtensions
    {
        public static string PrettifyJsonString(this string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return string.Empty;
            }

            var doc = JsonDocument.Parse(json);

            var options = new JsonWriterOptions
            {
                Indented = true
            };

            using var stream = new MemoryStream();
            using (var writer = new Utf8JsonWriter(stream, options))
            {
                doc.WriteTo(writer);
            }

            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }
}

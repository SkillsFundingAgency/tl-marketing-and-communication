using System.Text.Json;
using FluentAssertions;
using sfa.Tl.Marketing.Communication.Application.Extensions;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Application.Extensions
{
    public class JsonExtensionsTests
    {
        private readonly JsonDocument _jsonDoc = JsonDocument.Parse(
            "{ " +
                "\"anElement\": {" +
                    "\"myInt32\": 123," +
                    "\"myInt64\": 1000000000," +
                    "\"myString\": \"my value\"" +
                "}" +
            "}");

        [Theory(DisplayName = "JsonElement.SafeGetInt32 Data Tests")]
        [InlineData("myInt32", 123)]
        [InlineData("myString", 0)]
        [InlineData("notANumber", 0)]
        public void JsonElementSafeGetInt32DataTests(string propertyName, int expectedResult)
        {
            var prop = _jsonDoc.RootElement.GetProperty("anElement");
            var result = prop.SafeGetInt32(propertyName);

            result.Should().Be(expectedResult);
        }

        [Theory(DisplayName = "JsonElement.SafeGetInt32 Data Tests")]
        [InlineData("myInt64", 1000000000)]
        [InlineData("myString", 0)]
        [InlineData("notANumber", 0)]
        public void JsonElementSafeGetInt64DataTests(string propertyName, int expectedResult)
        {
            var prop = _jsonDoc.RootElement.GetProperty("anElement");
            var result = prop.SafeGetInt64(propertyName);

            result.Should().Be(expectedResult);
        }

        [Theory(DisplayName = "JsonElement.SafeGetString Data Tests")]
        [InlineData("myString", "my value")]
        [InlineData("myInt32", null)]
        [InlineData("notAString", null)]
        public void JsonElementSafeGetStringDataTests(string propertyName, string expectedResult)
        {
            var prop = _jsonDoc.RootElement.GetProperty("anElement");
            var result = prop.SafeGetString(propertyName);

            result.Should().Be(expectedResult);
        }
    }
}

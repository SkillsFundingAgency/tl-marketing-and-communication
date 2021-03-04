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
                    "\"myPositiveDouble\": 100.999," +
                    "\"myNegativeDouble\": -100.999," +
                    "\"myString\": \"my value\"" +
                "}" +
            "}");

        [Theory(DisplayName = "JsonElement.SafeGetInt32 Data Tests")]
        [InlineData("myInt32", 123)]
        [InlineData("myPositiveDouble", 0)]
        [InlineData("myString", 0)]
        [InlineData("notANumber", 0)]
        public void JsonElementSafeGetInt32DataTests(string propertyName, int expectedResult)
        {
            var prop = _jsonDoc.RootElement.GetProperty("anElement");
            var result = prop.SafeGetInt32(propertyName);

            result.Should().BeOfType(typeof(int));
            result.Should().Be(expectedResult);
        }

        [Theory(DisplayName = "JsonElement.SafeGetInt64 Data Tests")]
        [InlineData("myInt32", 123)]
        [InlineData("myInt64", 1000000000)]
        [InlineData("myPositiveDouble", 0)]
        [InlineData("myString", 0)]
        [InlineData("notANumber", 0)]
        public void JsonElementSafeGetInt64DataTests(string propertyName, long expectedResult)
        {
            var prop = _jsonDoc.RootElement.GetProperty("anElement");
            var result = prop.SafeGetInt64(propertyName);

            result.Should().BeOfType(typeof(long));
            result.Should().Be(expectedResult);
        }

        [Theory(DisplayName = "JsonElement.SafeGetDouble Data Tests")]
        [InlineData("myPositiveDouble", 100.999)]
        [InlineData("myNegativeDouble", -100.999)]
        [InlineData("myInt64", 1000000000)]
        [InlineData("myString", 0)]
        [InlineData("notANumber", 0)]
        public void JsonElementSafeGetDoubleDataTests(string propertyName, double expectedResult)
        {
            var prop = _jsonDoc.RootElement.GetProperty("anElement");
            var result = prop.SafeGetDouble(propertyName);

            result.Should().Be(expectedResult);
        }

        [Theory(DisplayName = "JsonElement.SafeGetString Data Tests")]
        [InlineData("myString", "my value")]
        [InlineData("myInt32", null)]
        [InlineData("myInt64", null)]
        [InlineData("myDouble", null)]
        [InlineData("notAString", null)]
        public void JsonElementSafeGetStringDataTests(string propertyName, string expectedResult)
        {
            var prop = _jsonDoc.RootElement.GetProperty("anElement");
            var result = prop.SafeGetString(propertyName);

            result.Should().Be(expectedResult);
        }
    }
}

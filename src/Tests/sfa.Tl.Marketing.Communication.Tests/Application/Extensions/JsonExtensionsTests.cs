using System.Text.Json;
using sfa.Tl.Marketing.Communication.Application.Extensions;

namespace sfa.Tl.Marketing.Communication.UnitTests.Application.Extensions;

public class JsonExtensionsTests
{
    private readonly JsonDocument _jsonDocument = JsonDocument.Parse(
        "{ " +
        "\"anElement\": {" +
        "\"myInt32\": 123," +
        "\"myInt64\": 1000000000," +
        "\"myPositiveDouble\": 100.999," +
        "\"myNegativeDouble\": -100.999," +
        "\"myPositiveDecimal\": 99.999," +
        "\"myNegativeDecimal\": -99.999," +
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
        var prop = _jsonDocument.RootElement.GetProperty("anElement");
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
        var prop = _jsonDocument.RootElement.GetProperty("anElement");
        var result = prop.SafeGetInt64(propertyName);

        result.Should().BeOfType(typeof(long));
        result.Should().Be(expectedResult);
    }

    [Theory(DisplayName = nameof(JsonExtensions.SafeGetDecimal) + " Data Tests")]
    [InlineData("myPositiveDecimal", 99.999)]
    [InlineData("myNegativeDecimal", -99.999)]
    [InlineData("myInt64", 1000000000)]
    [InlineData("myString", 0)]
    [InlineData("notANumber", 0)]
    public void JsonElement_SafeGetDecimal_Data_Tests(string propertyName, decimal expectedResult)
    {
        var prop = _jsonDocument.RootElement.GetProperty("anElement");
        var result = prop.SafeGetDecimal(propertyName);

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
        var prop = _jsonDocument.RootElement.GetProperty("anElement");
        var result = prop.SafeGetDouble(propertyName);

        result.Should().Be(expectedResult);
    }

    [Theory(DisplayName = "JsonElement.SafeGetDouble with Default Data Tests")]
    [InlineData("myPositiveDouble", 1, 100.999)]
    [InlineData("myNegativeDouble", -1, -100.999)]
    [InlineData("notANumber", 10, 10)]
    public void JsonElementSafeGetDoubleDefaultDataTests(string propertyName, double defaultValue, double expectedResult)
    {
        var prop = _jsonDocument.RootElement.GetProperty("anElement");
        var result = prop.SafeGetDouble(propertyName, defaultValue);

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
        var prop = _jsonDocument.RootElement.GetProperty("anElement");
        var result = prop.SafeGetString(propertyName);

        result.Should().Be(expectedResult);
    }

    [Theory(DisplayName = nameof(JsonExtensions.SafeGetString) + " with maxLength Data Tests")]
    [InlineData("myString", "my value", 10)]
    [InlineData("myString", "my value", 8)]
    [InlineData("myString", "my val", 6)]
    [InlineData("myInt32", null, 100)]
    [InlineData("myInt64", null, 100)]
    [InlineData("myDouble", null, 100)]
    [InlineData("notAString", null, 100)]
    [InlineData("myNull", null, 100)]
    public void JsonElement_SafeGetString_With_Max_Length_Data_Tests(string propertyName, string expectedResult, int maxLength)
    {
        var prop = _jsonDocument.RootElement.GetProperty("anElement");
        var result = prop.SafeGetString(propertyName, maxLength);

        result.Should().Be(expectedResult);
    }

    [Fact]
    public void JsonElement_SafeGetDouble_With_Undefined_Element_Returns_Zero()
    {
        var prop = _jsonDocument.RootElement.TryGetProperty("notAnElement", out var el);
        prop.Should().BeFalse("precondition - TryGetProperty should have failed");

        var result = el.SafeGetDouble("myPositiveDouble");
        result.Should().Be(0);
    }

    [Fact]
    public void JsonElement_SafeGetDecimal_With_Undefined_Element_Returns_Zero()
    {
        var prop = _jsonDocument.RootElement.TryGetProperty("notAnElement", out var el);
        prop.Should().BeFalse("precondition - TryGetProperty should have failed");

        var result = el.SafeGetDecimal("myPositiveDecimal");
        result.Should().Be(0);
    }

    [Fact]
    public void JsonElement_SafeGetInt32_With_Undefined_Element_Returns_Zero()
    {
        var prop = _jsonDocument.RootElement.TryGetProperty("notAnElement", out var el);
        prop.Should().BeFalse("precondition - TryGetProperty should have failed");

        var result = el.SafeGetInt32("myInt32");
        result.Should().Be(0);
    }

    [Fact]
    public void JsonElement_SafeGetInt64_With_Undefined_Element_Returns_Zero()
    {
        var prop = _jsonDocument.RootElement.TryGetProperty("notAnElement", out var el);
        prop.Should().BeFalse("precondition - TryGetProperty should have failed");

        var result = el.SafeGetInt64("myInt64");
        result.Should().Be(0);
    }

    [Fact]
    public void JsonElement_SafeGetString_With_Undefined_Element_Returns_Null()
    {
        var prop = _jsonDocument.RootElement.TryGetProperty("notAnElement", out var el);
        prop.Should().BeFalse("precondition - TryGetProperty should have failed");

        var result = el.SafeGetString("myString");
        result.Should().Be(null);
    }
}
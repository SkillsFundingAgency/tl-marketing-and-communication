using FluentAssertions;
using sfa.Tl.Marketing.Communication.Models.Extensions;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Model.Extensions;

public class StringExtensionsTests
{
    [Theory(DisplayName = "String.SafeGetString Data Tests")]
    [InlineData(null, null)]
    [InlineData("", "")]
    [InlineData("hello world", "Hello World")]
    [InlineData("ten Out Of 10", "Ten Out of 10")]
    [InlineData("Abingdon And Witney College", "Abingdon and Witney College")]
    [InlineData("Abingdon and Witney College", "Abingdon and Witney College")]
    [InlineData("abingdon and witney college", "Abingdon and Witney College")]
    [InlineData("Design, surveying and planning for Construction", "Design, Surveying and Planning for Construction")]
    [InlineData("Building services engineering for construction", "Building Services Engineering for Construction")]
    [InlineData("Bob's burger's", "Bob's Burger's")]
    [InlineData("Bob’s burger’s", "Bob’s Burger’s")]
    public void StringToTitleCaseDataTests(string input, string expectedResult)
    {
        var result = input.ToTitleCase();

        result.Should().Be(expectedResult);
    }

    [Theory(DisplayName = "String.ToLetterOrDigit Data Tests")]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("cv1 2wt", "cv12wt")]
    [InlineData("cv1-2wt", "cv12wt")]
    public void StringToLetterOrDigitDataTests(string input, string expectedResult)
    {
        var result = input.ToLetterOrDigit();

        result.Should().Be(expectedResult);
    }
}
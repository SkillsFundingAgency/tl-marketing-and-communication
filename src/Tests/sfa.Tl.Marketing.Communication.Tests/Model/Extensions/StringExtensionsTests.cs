using sfa.Tl.Marketing.Communication.Models.Extensions;

namespace sfa.Tl.Marketing.Communication.UnitTests.Model.Extensions;

public class StringExtensionsTests
{
    [Theory(DisplayName = nameof(StringExtensions.IsPostcode) + " Data Tests")]
    [InlineData("CV1 2WT", true)]
    [InlineData("cv1 2wt", true)]
    [InlineData("OXX 9XX", false)]
    [InlineData("Cov", false)]
    [InlineData("Coventry", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void String_IsPostcode(string input, bool expectedResult)
    {
        var result = input.IsPostcode();
        result.Should().Be(expectedResult);
    }

    [Theory(DisplayName = nameof(StringExtensions.IsPartialPostcode) + " Data Tests")]
    [InlineData("CV1 2WT", false)]
    [InlineData("CV1", true)]
    [InlineData("cv1", true)]
    [InlineData("L1", true)]
    [InlineData("OXX", false)]
    [InlineData("Cov", false)]
    [InlineData("Coventry", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void String_IsPartialPostcode(string input, bool expectedResult)
    {
        var result = input.IsPartialPostcode();
        result.Should().Be(expectedResult);
    }

    [Theory(DisplayName = nameof(StringExtensions.IsFullOrPartialPostcode) + " Data Tests")]
    [InlineData("CV1 2WT", true)]
    [InlineData("cv1 2wt", true)]
    [InlineData("OXX 9XX", false)]
    [InlineData("CV1", true)]
    [InlineData("Cov", false)]
    [InlineData("Coventry", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    [InlineData("cv1", true)]
    [InlineData("L1", true)]
    [InlineData("OXX", false)]
    public void String_IsFullOrPartialPostcode(string input, bool expectedResult)
    {
        var result = input.IsFullOrPartialPostcode();
        result.Should().Be(expectedResult);
    }

    [Theory(DisplayName = nameof(StringExtensions.ToSearchableString) + " Data Tests")]
    // ReSharper disable StringLiteralTypo
    [InlineData(null, null)]
    [InlineData("CV1 2WT", "cv12wt")]
    [InlineData("St. Albans", "stalbans")]
    [InlineData("Colton & the Ridwares", "coltonandtheridwares")]
    [InlineData("Coates (Cotswold),	Gloucestershire", "coatescotswoldgloucestershire")]
    [InlineData("Coleorton/Griffydam, Leicestershire", "coleortongriffydamleicestershire")]
    [InlineData("Collett's Green", "collettsgreen")]
    [InlineData("Newcastle-under-Lyme, Staffordshire", "newcastleunderlymestaffordshire")]
    [InlineData("Westward Ho!", "westwardho")]
    [InlineData("Oakthorpe & Donisthorpe", "oakthorpeanddonisthorpe")]
    [InlineData("Bede, Tyne & Wear", "bedetyneandwear")]
    [InlineData("Bishop's Castle, Shropshire", "bishopscastleshropshire")]
    // ReSharper restore StringLiteralTypo
    public void String_ToSearchableString_Data_Tests(string input, string expectedResult)
    {
        var result = input.ToSearchableString();

        result.Should().Be(expectedResult);
    }

    [Theory(DisplayName = nameof(StringExtensions.ToTitleCase) + " Data Tests")]
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

    [Theory(DisplayName = nameof(StringExtensions.ToLetterOrDigit) + "  Data Tests")]
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
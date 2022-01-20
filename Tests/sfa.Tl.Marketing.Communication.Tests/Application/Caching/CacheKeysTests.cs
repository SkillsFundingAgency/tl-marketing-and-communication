using System;
using FluentAssertions;
using sfa.Tl.Marketing.Communication.Application.Caching;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Application.Caching
{
    public class CacheKeysTests
    {
        [Theory(DisplayName = nameof(CacheKeys.PostcodeKey) + " Data Tests")]
        [InlineData("cv12wt", "POSTCODE__CV12WT")]
        [InlineData("CV1 2WT", "POSTCODE__CV12WT")]
        public void Postcode_Key_Returns_Expected_Value(string postcode, string expectedKey)
        {
            var key = CacheKeys.PostcodeKey(postcode);
            key.Should().Be(expectedKey);
        }

        [Fact]
        public void PostcodeKey_Throws_Exception_For_Null_Postcode()
        {
            Action act = () => CacheKeys.PostcodeKey(null);

            act.Should().Throw<ArgumentNullException>();

            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("postcode");
        }

        [Fact]
        public void PostcodeKey_Throws_Exception_For_Empty_Postcode()
        {
            Action act = () => CacheKeys.PostcodeKey("");

            act.Should().Throw<ArgumentException>();

            act.Should().Throw<ArgumentException>()
                .WithMessage("A non-empty postcode is required*")
                .WithParameterName("postcode");
        }
    }
}

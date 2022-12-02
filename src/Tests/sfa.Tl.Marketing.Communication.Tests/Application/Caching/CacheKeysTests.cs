using System;
using FluentAssertions;
using sfa.Tl.Marketing.Communication.Application.Caching;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Application.Caching;

public class CacheKeysTests
{
    [Theory(DisplayName = nameof(CacheKeys.PostcodeKey) + " Data Tests")]
    [InlineData("cv12wt", "POSTCODE__CV12WT")]
    [InlineData("CV1 2WT", "POSTCODE__CV12WT")]
    public void PostcodeKey_Returns_Expected_Value(string postcode, string expectedKey)
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

    [Theory(DisplayName = nameof(CacheKeys.TownPartitionKey) + " Data Tests")]
    [InlineData("COV", "TOWNS__COV")]
    [InlineData("C", "TOWNS__C")]
    public void TownPartitionKey_Returns_Expected_Value(string partitionKey, string expectedKey)
    {
        var key = CacheKeys.TownPartitionKey(partitionKey);
        key.Should().Be(expectedKey);
    }

    [Fact]
    public void TownPartitionKey_Throws_Exception_For_Null_Partition_Key()
    {
        Action act = () => CacheKeys.TownPartitionKey(null);

        act.Should().Throw<ArgumentNullException>();

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("partitionKey");
    }

    [Fact]
    public void TownPartitionKey_Throws_Exception_For_Empty_Partition_Key()
    {
        Action act = () => CacheKeys.TownPartitionKey("");

        act.Should().Throw<ArgumentException>();

        act.Should().Throw<ArgumentException>()
            .WithMessage("A non-empty partition key is required*")
            .WithParameterName("partitionKey");
    }
}
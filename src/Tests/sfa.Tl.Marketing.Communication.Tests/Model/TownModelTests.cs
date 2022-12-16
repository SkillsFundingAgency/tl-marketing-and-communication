using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.UnitTests.Model;

public class TownModelTests
{
    [Theory(DisplayName = nameof(Town.DisplayName) + " Data Tests")]
    [InlineData(null, null, null, null)]
    [InlineData("My Town", null, null, "My Town")]
    [InlineData("My Town", "Eastern County", null, "My Town, Eastern County")]
    [InlineData("My Town", null, "Central Authority", "My Town, Central Authority")]
    [InlineData("My Town", "Eastern County", "Central Authority", "My Town, Eastern County")]
    public void Town_DisplayName(string name, string county, string localAuthority, string expected)
    {
        var town = new Town
        {
            Name = name,
            County = county,
            LocalAuthority = localAuthority
        };

        var result = town.DisplayName;
        result.Should().Be(expected);
    }
}
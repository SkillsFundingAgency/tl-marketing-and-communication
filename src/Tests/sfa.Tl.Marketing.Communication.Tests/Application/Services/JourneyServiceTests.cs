using FluentAssertions;
using sfa.Tl.Marketing.Communication.Application.Services;
using sfa.Tl.Marketing.Communication.Tests.Common.Extensions;
using sfa.Tl.Marketing.Communication.UnitTests.Builders;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Application.Services;

public class JourneyServiceTests
{
    [Fact]
    public void JourneyService_Constructor_Guards_Against_NullParameters()
    {
        typeof(JourneyService)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void JourneyService_Returns_Correct_Link()
    {
        var service = new JourneyService();

        var result = service.GetDirectionsLink(
            "B91 1SB",
            new ProviderLocationBuilder().Build());

        result.Should().Be("https://www.google.com/maps/dir/?api=1&origin=B91+1SB&destination=CV1+2WT&travelmode=transit");
    }
}
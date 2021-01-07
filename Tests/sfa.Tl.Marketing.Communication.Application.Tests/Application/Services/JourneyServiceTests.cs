using FluentAssertions;
using sfa.Tl.Marketing.Communication.Application.Services;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Application.Services
{
    public class JourneyServiceTests
    {
        [Fact]
        public void JourneyService_Returns_Correct_Link()
        {
            var service = new JourneyService();

            var result = service.GetDirectionsLink(
                "CV1 2WT", 52.400997, -1.508122,
                "B91 1SB", 52.409568, -1.792148);

            result.Should().Be("https://www.google.com/maps/dir/?api=1&origin=52.400997,-1.508122&destination=52.409568,-1.792148&travelmode=transit");
        }
    }
}

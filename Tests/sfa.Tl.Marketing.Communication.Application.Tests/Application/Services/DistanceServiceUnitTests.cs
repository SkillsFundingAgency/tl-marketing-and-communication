using FluentAssertions;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Application.Services;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Application.Services
{
    public class DistanceServiceUnitTests
    {
        private readonly IDistanceService _distanceService;

        public DistanceServiceUnitTests()
        {
            _distanceService = new DistanceService();
        }

        [Theory]
        [InlineData(52.05801, -0.784115, 52.133347, -0.468552, 14.375097500047632)]
        [InlineData(52.05801, -0.784115, 52.486942, -0.692251, 29.89913575084169)]
        [InlineData(52.05801, -0.784115, 53.587875, -2.294975, 123.12734511742885)]
        [InlineData(52.05801, -0.784115, 54.903545, -1.384952, 198.21372605959934)]
        public void CalculateInMiles_Calculate_Distance_In_Miles(double lat1, double lon1, double lat2, double lon2, double expected)
        {
            // Arrange
            // Act
            var actual = _distanceService.CalculateInMiles(lat1, lon1, lat2, lon2);

            // Assert
            actual.Should().Be(expected);
        }
    }
}

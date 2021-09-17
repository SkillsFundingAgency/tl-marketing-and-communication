using System;
using FluentAssertions;
using sfa.Tl.Marketing.Communication.Application.Extensions;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Application.Extensions
{
    public class BusinessRuleExtensionsTests
    {
        [Theory(DisplayName = nameof(BusinessRuleExtensions.IsAvailableAtDate) + " Data Tests")]
        [InlineData(2020, "2020-12-31", true)]
        [InlineData(2021, "2020-12-31", false)]
        [InlineData(2021, "2021-08-31", false)]
        [InlineData(2021, "2021-09-01", true)]
        [InlineData(2021, "2022-09-01", true)]
        [InlineData(2022, "2022-08-31", false)]
        [InlineData(2022, "2022-09-01", true)]
        [InlineData(2023, "2023-08-31", false)]
        [InlineData(2023, "2023-09-01", true)]
        public void DeliveryYear_IsAvailableAtDate_Data_Tests(short deliveryYear, string currentDate,
            bool expectedResult)
        {
            var today = DateTime.Parse(currentDate);

            var result = deliveryYear.IsAvailableAtDate(today);
            result.Should().Be(expectedResult);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using sfa.Tl.Marketing.Communication.Extensions;
using sfa.Tl.Marketing.Communication.Application.Extensions;
using sfa.Tl.Marketing.Communication.Models;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Application.Extensions
{
    public class BusinessRuleExtensionsTests
    {
        [Theory(DisplayName = nameof(Communication.Application.Extensions.BusinessRuleExtensions.IsAvailableAtDate) + " Data Tests")]
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

        [Fact]
        public void MergeAvailableDeliveryYears_Before_Available_Returns_Expected_Result()
        {
            var today = DateTime.Parse("2021-08-31");

            var providerLocations = BuildProviderLocationViewModelList();

            providerLocations.MergeAvailableDeliveryYears(today);

            providerLocations[0].DeliveryYears.Count.Should().Be(2);
            providerLocations[0].DeliveryYears[0].IsAvailableNow.Should().BeFalse();
            providerLocations[0].DeliveryYears[1].IsAvailableNow.Should().BeFalse();
        }

        [Fact]
        public void MergeAvailableDeliveryYears_After_Available_Returns_Expected_Result()
        {
            var today = DateTime.Parse("2021-09-01");

            var providerLocations = BuildProviderLocationViewModelList();

            providerLocations.MergeAvailableDeliveryYears(today);

            providerLocations[0].DeliveryYears.Count.Should().Be(1);
            providerLocations[0].DeliveryYears[0].IsAvailableNow.Should().BeTrue();
            providerLocations[0].DeliveryYears[0].Qualifications.Count().Should().Be(2);

            var qualifications = providerLocations[0].DeliveryYears[0].Qualifications.ToList();
            qualifications[0].Id.Should().Be(1);
            qualifications[0].Name.Should().Be("Test Qualification 1");
            qualifications[1].Id.Should().Be(2);
            qualifications[1].Name.Should().Be("Test Qualification 2");
        }

        public IList<ProviderLocationViewModel> BuildProviderLocationViewModelList() =>
            new List<ProviderLocationViewModel>
            {
                new ProviderLocationViewModel()
                {
                    ProviderName = "Test Provider",
                    Name = "Test Provider",
                    Postcode = "CV1 2WT",
                    DeliveryYears = BuildDeliveryYearViewModelList()
                }
            };

        public IList<DeliveryYearViewModel> BuildDeliveryYearViewModelList() =>
            new List<DeliveryYearViewModel>
            {
                new DeliveryYearViewModel
                {
                    Year = 2021,
                    Qualifications = new List<QualificationViewModel>
                    {
                        new() { Id = 1, Name = "Test Qualification 1" }
                    }
                },
                new DeliveryYearViewModel
                {
                    Year = 2021,
                    Qualifications = new List<QualificationViewModel>
                    {
                        new() { Id = 2, Name = "Test Qualification 2" }
                    }
                }
            };
    }
}

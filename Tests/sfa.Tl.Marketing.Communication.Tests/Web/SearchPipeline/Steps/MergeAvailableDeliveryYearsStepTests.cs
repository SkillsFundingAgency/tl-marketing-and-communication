using System;
using FluentAssertions;
using NSubstitute;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models;
using sfa.Tl.Marketing.Communication.SearchPipeline;
using sfa.Tl.Marketing.Communication.SearchPipeline.Steps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Web.SearchPipeline.Steps
{
    public class MergeAvailableDeliveryYearsStepTests
    {
        [Fact]
        public async Task MergeAvailableDeliveryYears_Before_Available_Returns_Expected_Result()
        {
            var dateTimeService = Substitute.For<IDateTimeService>();
            dateTimeService.Today.Returns(DateTime.Parse("2021-08-31"));

            var providerLocations = BuildProviderLocationViewModelList();

            var viewModel = new FindViewModel
            {
                ProviderLocations = providerLocations
            };

            var context = new SearchContext(viewModel);

            var searchStep = new MergeAvailableDeliveryYearsStep(dateTimeService);

            await searchStep.Execute(context);
            
            providerLocations[0].DeliveryYears.Count.Should().Be(2);
            providerLocations[0].DeliveryYears[0].IsAvailableNow.Should().BeFalse();
            providerLocations[0].DeliveryYears[1].IsAvailableNow.Should().BeFalse();
        }

        [Fact]
        public async Task MergeAvailableDeliveryYears_After_Available_Returns_Expected_Result()
        {
            var dateTimeService = Substitute.For<IDateTimeService>();
            dateTimeService.Today.Returns(DateTime.Parse("2021-09-01"));

            var providerLocations = BuildProviderLocationViewModelList();

            var viewModel = new FindViewModel
            {
                ProviderLocations = providerLocations
            };

            var context = new SearchContext(viewModel);

            var searchStep = new MergeAvailableDeliveryYearsStep(dateTimeService);

            await searchStep.Execute(context);

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
                new()
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
                new()
                {
                    Year = 2021,
                    Qualifications = new List<QualificationViewModel>
                    {
                        new() { Id = 1, Name = "Test Qualification 1" }
                    }
                },
                new()
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

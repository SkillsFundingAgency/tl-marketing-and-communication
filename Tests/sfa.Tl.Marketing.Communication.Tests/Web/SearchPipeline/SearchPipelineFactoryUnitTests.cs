using System.Collections.Generic;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.SearchPipeline;
using sfa.Tl.Marketing.Communication.SearchPipeline.Steps;
using System.Linq;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Web.SearchPipeline
{
    public class SearchPipelineFactoryUnitTests
    {
        private readonly ISearchPipelineFactory _factory;

        public SearchPipelineFactoryUnitTests()
        {
            var dateTimeService = Substitute.For<IDateTimeService>();

            var mapper = Substitute.For<IMapper>();

            var providerSearchService = Substitute.For<IProviderSearchService>();

            var searchSteps = new List<ISearchStep>
            {
                new CalculateNumberOfItemsToShowStep(),
                new GetQualificationsStep(providerSearchService),
                new LoadSearchPageWithNoResultsStep(),
                new MergeAvailableDeliveryYearsStep(dateTimeService),
                new PerformSearchStep(providerSearchService, dateTimeService, mapper),
                new ValidatePostcodeStep(providerSearchService)
            };

            _factory = new SearchPipelineFactory(searchSteps);
        }

        [Fact]
        public void Factory_Validate_Order_Of_SearchPipeline_Steps()
        {
            var providerSearchService = Substitute.For<IProviderSearchService>();
            var mapper = Substitute.For<IMapper>();

            var steps = _factory.GetSearchSteps(providerSearchService, mapper).ToArray();

            steps.Length.Should().Be(6);
            steps[0].GetType().Name.Should().Be(nameof(GetQualificationsStep));
            steps[1].GetType().Name.Should().Be(nameof(LoadSearchPageWithNoResultsStep));
            steps[2].GetType().Name.Should().Be(nameof(ValidatePostcodeStep));
            steps[3].GetType().Name.Should().Be(nameof(CalculateNumberOfItemsToShowStep));
            steps[4].GetType().Name.Should().Be(nameof(PerformSearchStep));
            steps[5].GetType().Name.Should().Be(nameof(MergeAvailableDeliveryYearsStep));
        }
    }
}

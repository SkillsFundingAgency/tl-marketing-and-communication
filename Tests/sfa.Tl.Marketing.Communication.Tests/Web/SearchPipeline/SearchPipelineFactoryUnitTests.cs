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
            _factory = new SearchPipelineFactory();
        }

        [Fact]
        public void Factory_Validate_Order_Of_SearchPipeline_Steps()
        {
            // Arrange
            var providerSearchService = Substitute.For<IProviderSearchService>();
            var mapper = Substitute.For<IMapper>();

            // Act
            var steps = _factory.GetSearchSteps(providerSearchService, mapper).ToArray();

            // Assert
            steps[0].GetType().Name.Should().Be(nameof(GetQualificationIdToSearchStep));
            steps[1].GetType().Name.Should().Be(nameof(GetQualificationsStep));
            steps[2].GetType().Name.Should().Be(nameof(LoadSearchPageWithNoResultsStep));
            steps[3].GetType().Name.Should().Be(nameof(ValidatePostcodeStep));
            steps[4].GetType().Name.Should().Be(nameof(CalculateNumberOfItemsToShowStep));
            steps[5].GetType().Name.Should().Be(nameof(PerformSearchStep));
        }
    }
}

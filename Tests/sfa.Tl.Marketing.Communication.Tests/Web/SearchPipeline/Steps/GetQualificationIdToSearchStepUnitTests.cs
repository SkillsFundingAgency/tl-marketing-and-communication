using FluentAssertions;
using sfa.Tl.Marketing.Communication.Models;
using sfa.Tl.Marketing.Communication.SearchPipeline;
using sfa.Tl.Marketing.Communication.SearchPipeline.Steps;
using System.Threading.Tasks;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Web.SearchPipeline.Steps
{
    public class GetQualificationIdToSearchStepUnitTests
    {
        private readonly ISearchStep _searchStep;

        public GetQualificationIdToSearchStepUnitTests()
        {
            _searchStep = new GetQualificationIdToSearchStep();
        }

        [Theory]
        [InlineData(null, 0)]
        [InlineData(0, 0)]
        [InlineData(42, 42)]
        public async Task Step_Setup_QualificationId_For_Search(int? qualificationId, int expected)
        {
            // Arrange
            var viewModel = new FindViewModel
            {
                SelectedQualificationId = qualificationId
            };
            var context = new SearchContext(viewModel);
            
            // Act
            await _searchStep.Execute(context);

            // Assert
            context.ViewModel.SelectedQualificationId.Should().Be(expected);
        }
    }
}

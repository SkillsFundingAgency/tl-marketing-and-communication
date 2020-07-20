using FluentAssertions;
using NSubstitute;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models;
using sfa.Tl.Marketing.Communication.Models.Dto;
using sfa.Tl.Marketing.Communication.SearchPipeline;
using sfa.Tl.Marketing.Communication.SearchPipeline.Steps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Web.SearchPipeline.Steps
{
    public class GetQualificationsStepUnitTests
    {
        private readonly IProviderSearchService _providerSearchService;
        private readonly ISearchStep _searchStep;

        public GetQualificationsStepUnitTests()
        {
            _providerSearchService = Substitute.For<IProviderSearchService>();
            _searchStep = new GetQualificationsStep(_providerSearchService);
        }

        [Fact]
        public async Task Step_Returns_SelectListItems_For_All_Qualifications_With_A_Selected_Qualification()
        {
            // Arrange
            int? selectedQualificationId = 3;

            var viewModel = new FindViewModel()
            {
                SelectedQualificationId = selectedQualificationId
            };

            var context = new SearchContext(viewModel);

            var qualifications = new List<Qualification>
            {
                new Qualification {Id = 1, Name = "Qualification 1" },
                new Qualification {Id = 2, Name = "Qualification 2" },
                new Qualification {Id = 3, Name = "Qualification 3" },
                new Qualification {Id = 4, Name = "Qualification 4" },
                new Qualification {Id = 5, Name = "Qualification 5" }
            };

            _providerSearchService.GetQualifications().Returns(qualifications);

            // Act
            await _searchStep.Execute(context);

            // Assert
            _providerSearchService.Received(1).GetQualifications();
            context.ViewModel.Qualifications.Count().Should().Be(qualifications.Count);
            context.ViewModel.Qualifications
                .Any(q => q.Value == selectedQualificationId.ToString() && q.Selected)
                .Should().BeTrue();
        }
    }
}

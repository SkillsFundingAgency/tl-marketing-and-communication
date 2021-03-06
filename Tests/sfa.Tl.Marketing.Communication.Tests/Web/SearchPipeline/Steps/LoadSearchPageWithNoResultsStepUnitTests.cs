﻿using FluentAssertions;
using sfa.Tl.Marketing.Communication.Models;
using sfa.Tl.Marketing.Communication.SearchPipeline;
using sfa.Tl.Marketing.Communication.SearchPipeline.Steps;
using System.Threading.Tasks;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Web.SearchPipeline.Steps
{
    public class LoadSearchPageWithNoResultsStepUnitTests
    {
        private readonly ISearchStep _searchStep;

        public LoadSearchPageWithNoResultsStepUnitTests()
        {
            _searchStep = new LoadSearchPageWithNoResultsStep();
        }

        [Fact]
        public async Task Step_Load_Find_Page_And_Make_Page_Ready_For_Search()
        {
            // Arrange
            const bool shouldSearch = false;
            var viewModel = new FindViewModel
            {
                ShouldSearch = shouldSearch
            };
            
            var context = new SearchContext(viewModel);

            // Act
            await _searchStep.Execute(context);

            // Assert
            context.ViewModel.ShouldSearch.Should().BeTrue();
            context.Continue.Should().BeFalse();
        }
    }
}

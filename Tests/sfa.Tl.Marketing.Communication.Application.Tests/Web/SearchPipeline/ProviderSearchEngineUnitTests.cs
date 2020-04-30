using AutoMapper;
using NSubstitute;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models;
using sfa.Tl.Marketing.Communication.SearchPipeline;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Web.SearchPipeline
{
    public class ProviderSearchEngineUnitTests
    {
        private readonly IProviderSearchService _providerSearchService;
        private readonly IMapper _mapper;
        private readonly ISearchPipelineFactory _searchPipelineFactory;
        private readonly IProviderSearchEngine _providerSearchEngine;

        public ProviderSearchEngineUnitTests()
        {
            _providerSearchService = Substitute.For<IProviderSearchService>();
            _searchPipelineFactory = Substitute.For<ISearchPipelineFactory>();
            _mapper = Substitute.For<IMapper>();
            _providerSearchEngine = new ProviderSearchEngine(_providerSearchService, _mapper, _searchPipelineFactory);
        }

        [Fact]
        public async Task Search_Execute_All_Steps()
        {
            // Arrange
            var viewModel = new FindViewModel();
            var context = new SearchContext(viewModel);
            
            _searchPipelineFactory.GetSearchContext(Arg.Is(viewModel)).Returns(context);
            var searchStep1 = Substitute.For<ISearchStep>();
            var searchStep2 = Substitute.For<ISearchStep>();
            var searchStep3 = Substitute.For<ISearchStep>();
            var steps = new List<ISearchStep>() 
            {
                searchStep1,
                searchStep2,
                searchStep3
            };

            _searchPipelineFactory.GetSearchSteps(_providerSearchService, _mapper).Returns(steps);

            // Act
            await _providerSearchEngine.Search(viewModel);

            // Assert
            await searchStep1.Received(1).Execute(context);
            await searchStep2.Received(1).Execute(context);
            await searchStep3.Received(1).Execute(context);
        }

        [Fact]
        public async Task Search_Will_Not_Execute_Steps_When_Continue_On_SearchContext_Is_False()
        {
            // Arrange
            var viewModel = new FindViewModel();
            var context = new SearchContext(viewModel)
            {
                Continue = false
            };
            _searchPipelineFactory.GetSearchContext(Arg.Is(viewModel)).Returns(context);
            var searchStep1 = Substitute.For<ISearchStep>();
            var searchStep2 = Substitute.For<ISearchStep>();
            var searchStep3 = Substitute.For<ISearchStep>();
            var steps = new List<ISearchStep>()
            {
                searchStep1,
                searchStep2,
                searchStep3
            };

            _searchPipelineFactory.GetSearchSteps(_providerSearchService, _mapper).Returns(steps);

            // Act
            await _providerSearchEngine.Search(viewModel);

            // Assert
            await searchStep1.Received(1).Execute(context);
            await searchStep2.Received(0).Execute(context);
            await searchStep3.Received(0).Execute(context);
        }
    }
}

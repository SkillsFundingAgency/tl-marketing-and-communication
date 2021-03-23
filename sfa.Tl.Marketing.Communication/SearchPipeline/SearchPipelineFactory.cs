using AutoMapper;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models;
using sfa.Tl.Marketing.Communication.SearchPipeline.Steps;
using System.Collections.Generic;

namespace sfa.Tl.Marketing.Communication.SearchPipeline
{
    public class SearchPipelineFactory : ISearchPipelineFactory
    {
        public ISearchContext GetSearchContext(FindViewModel viewModel)
        {
            return new SearchContext(viewModel);
        }

        public IEnumerable<ISearchStep> GetSearchSteps(IProviderSearchService providerSearchService, IMapper mapper)
        {
            var searchSteps = new List<ISearchStep>
            {
                new GetQualificationsStep(providerSearchService),
                new LoadSearchPageWithNoResultsStep(),
                new ValidatePostcodeStep(providerSearchService),
                new CalculateNumberOfItemsToShowStep(),
                new PerformSearchStep(providerSearchService, mapper)
            };

            return searchSteps;
        }
    }
}

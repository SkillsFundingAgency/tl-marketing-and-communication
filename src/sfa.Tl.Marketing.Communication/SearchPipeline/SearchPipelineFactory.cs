using System;
using AutoMapper;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models;
using sfa.Tl.Marketing.Communication.SearchPipeline.Steps;
using System.Collections.Generic;
using System.Linq;

namespace sfa.Tl.Marketing.Communication.SearchPipeline;

public class SearchPipelineFactory : ISearchPipelineFactory
{
    private readonly IList<ISearchStep> _searchSteps;

    public SearchPipelineFactory(
        IEnumerable<ISearchStep> searchSteps)
    {
        if(searchSteps is null) throw new ArgumentNullException(nameof(searchSteps));
        _searchSteps = searchSteps.ToList();
    }

    public ISearchContext GetSearchContext(FindViewModel viewModel)
    {
        return new SearchContext(viewModel);
    }

    public IEnumerable<ISearchStep> GetSearchSteps(IProviderSearchService providerSearchService, IMapper mapper)
    {
        return new List<ISearchStep>
        {
            _searchSteps.OfType<GetQualificationsStep>().Single(),
            _searchSteps.OfType<LoadSearchPageWithNoResultsStep>().Single(),
            _searchSteps.OfType<ValidatePostcodeStep>().Single(),
            _searchSteps.OfType<CalculateNumberOfItemsToShowStep>().Single(),
            _searchSteps.OfType<PerformSearchStep>().Single(),
            _searchSteps.OfType<MergeAvailableDeliveryYearsStep>().Single()
        };
    }
}
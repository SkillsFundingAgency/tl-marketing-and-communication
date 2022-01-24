using AutoMapper;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models;
using System.Collections.Generic;

namespace sfa.Tl.Marketing.Communication.SearchPipeline;

public interface ISearchPipelineFactory
{
    ISearchContext GetSearchContext(FindViewModel viewModel);
    IEnumerable<ISearchStep> GetSearchSteps(IProviderSearchService providerSearchService, IMapper mapper);
}
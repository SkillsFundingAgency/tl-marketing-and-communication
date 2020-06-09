using AutoMapper;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models;
using System.Threading.Tasks;

namespace sfa.Tl.Marketing.Communication.SearchPipeline
{
    public class ProviderSearchEngine : IProviderSearchEngine
    {
        private readonly IProviderSearchService _providerSearchService;
        private readonly IMapper _mapper;
        private readonly ISearchPipelineFactory _searchPipelineFactory;

        public ProviderSearchEngine(IProviderSearchService providerSearchService, IMapper mapper, ISearchPipelineFactory searchPipelineFactory)
        {
            _providerSearchService = providerSearchService;
            _searchPipelineFactory = searchPipelineFactory;
            _mapper = mapper;
        }

        public async Task<FindViewModel> Search(FindViewModel viewModel)
        {
            var context = _searchPipelineFactory.GetSearchContext(viewModel);
            var searchSteps = _searchPipelineFactory.GetSearchSteps(_providerSearchService, _mapper);

            foreach (var searchStep in searchSteps)
            {
                await searchStep.Execute(context);

                if (!context.Continue)
                {
                    break;
                }
            }

            return context.ViewModel;
        }
    }
}

using System.Diagnostics;
using AutoMapper;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace sfa.Tl.Marketing.Communication.SearchPipeline
{
    public class ProviderSearchEngine : IProviderSearchEngine
    {
        private readonly IProviderSearchService _providerSearchService;
        private readonly IMapper _mapper;
        private readonly ISearchPipelineFactory _searchPipelineFactory;
        private readonly ILogger<ProviderSearchEngine> _logger;

        public ProviderSearchEngine(
            IProviderSearchService providerSearchService, 
            IMapper mapper, 
            ISearchPipelineFactory searchPipelineFactory,
            ILogger<ProviderSearchEngine> logger)
        {
            _providerSearchService = providerSearchService;
            _searchPipelineFactory = searchPipelineFactory;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<FindViewModel> Search(FindViewModel viewModel)
        {
            var stopwatch = Stopwatch.StartNew();

            var context = _searchPipelineFactory.GetSearchContext(viewModel);
            var searchSteps = _searchPipelineFactory.GetSearchSteps(_providerSearchService, _mapper);

            var innerStopwatch = new Stopwatch();
            foreach (var searchStep in searchSteps)
            {
                innerStopwatch.Restart();

                await searchStep.Execute(context);

                innerStopwatch.Stop();
                _logger.LogInformation("Search step {step} - {milliseconds}ms, {ticks} ticks", 
                    searchStep.GetType().Name, 
                    innerStopwatch.ElapsedMilliseconds,
                    innerStopwatch.ElapsedTicks);

                if (!context.Continue)
                {
                    break;
                }
            }

            stopwatch.Stop();
            _logger.LogInformation("Search took {elapsedMilliseconds} ms, {elapsedTicks} ticks", stopwatch.ElapsedMilliseconds, stopwatch.ElapsedTicks);

            return context.ViewModel;
        }
    }
}

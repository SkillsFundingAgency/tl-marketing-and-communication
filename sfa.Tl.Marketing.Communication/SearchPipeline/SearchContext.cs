using sfa.Tl.Marketing.Communication.Models;

namespace sfa.Tl.Marketing.Communication.SearchPipeline
{
    public class SearchContext : ISearchContext
    {
        public SearchContext(FindViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public FindViewModel ViewModel { get; }

        public bool Continue { get; set; } = true;
    }
}

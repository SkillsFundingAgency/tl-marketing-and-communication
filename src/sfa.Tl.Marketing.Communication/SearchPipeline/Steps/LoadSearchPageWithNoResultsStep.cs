using System.Threading.Tasks;

namespace sfa.Tl.Marketing.Communication.SearchPipeline.Steps
{
    public class LoadSearchPageWithNoResultsStep : ISearchStep
    {
        public Task Execute(ISearchContext context)
        {
            if (!context.ViewModel.ShouldSearch)
            {
                context.ViewModel.ShouldSearch = true;
                context.Continue = false;
            }

            return Task.CompletedTask;
        }
    }
}

using System.Threading.Tasks;

namespace sfa.Tl.Marketing.Communication.SearchPipeline.Steps
{
    public class GetQualificationIdToSearchStep : ISearchStep
    {
        public Task Execute(ISearchContext context)
        {
            context.ViewModel.SelectedQualificationId ??= 0;
            return Task.CompletedTask;
        }
    }
}

using System.Threading.Tasks;

namespace sfa.Tl.Marketing.Communication.SearchPipeline.Steps
{
    public class CalculateNumberOfItemsToShowStep : ISearchStep
    {
        public async Task Execute(ISearchContext context)
        {
            if (context.ViewModel.SubmitType == "search")
            {
                context.ViewModel.NumberOfItemsToShow = 5;
                context.ViewModel.SelectedItemIndex = 0;
                context.ViewModel.TotalRecordCount = 0;
            }

            if (context.ViewModel.SearchedQualificationId != context.ViewModel.SelectedQualificationId.Value)
            {
                context.ViewModel.NumberOfItemsToShow = 5;
                context.ViewModel.SelectedItemIndex = 0;
            }

            if (!context.ViewModel.NumberOfItemsToShow.HasValue)
            {
                context.ViewModel.NumberOfItemsToShow = 5;
            }

            if (!context.ViewModel.TotalRecordCount.HasValue)
            {
                context.ViewModel.SelectedItemIndex = 0;
            }

            if (context.ViewModel.TotalRecordCount.HasValue && context.ViewModel.SearchedQualificationId == context.ViewModel.SelectedQualificationId.Value)
            {
                var remainingCount = context.ViewModel.TotalRecordCount.Value - context.ViewModel.NumberOfItemsToShow.Value;
                
                if (remainingCount >= 5)
                {
                    context.ViewModel.SelectedItemIndex = context.ViewModel.NumberOfItemsToShow.Value;
                    context.ViewModel.NumberOfItemsToShow = context.ViewModel.NumberOfItemsToShow.Value + 5;
                }
                else if (remainingCount > 0)
                {
                    context.ViewModel.SelectedItemIndex = context.ViewModel.NumberOfItemsToShow.Value;
                    context.ViewModel.NumberOfItemsToShow = context.ViewModel.TotalRecordCount.Value;
                }
            }
        }
    }
}

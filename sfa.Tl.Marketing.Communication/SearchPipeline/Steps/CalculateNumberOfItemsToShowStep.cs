using sfa.Tl.Marketing.Communication.Constants;
using System.Threading.Tasks;
using sfa.Tl.Marketing.Communication.Models;

namespace sfa.Tl.Marketing.Communication.SearchPipeline.Steps
{
    public class CalculateNumberOfItemsToShowStep : ISearchStep
    {
        public Task Execute(ISearchContext context)
        {
            if (context.ViewModel.SubmitType != SearchSubmitType.Next)
            {
                context.ViewModel.NumberOfItemsToShow = AppConstants.DefaultNumberOfItemsToShow;
                context.ViewModel.SelectedItemIndex = 0;
                context.ViewModel.TotalRecordCount = 0;
            }

            context.ViewModel.NumberOfItemsToShow ??= AppConstants.DefaultNumberOfItemsToShow;

            if (!context.ViewModel.TotalRecordCount.HasValue)
            {
                context.ViewModel.SelectedItemIndex = 0;
            }

            if (context.ViewModel.SelectedQualificationId.HasValue && 
                context.ViewModel.SearchedQualificationId != context.ViewModel.SelectedQualificationId.Value)
            {
                context.ViewModel.NumberOfItemsToShow = AppConstants.DefaultNumberOfItemsToShow;
                context.ViewModel.SelectedItemIndex = 0;
            }

            if (context.ViewModel.TotalRecordCount.HasValue && 
                context.ViewModel.SelectedQualificationId.HasValue && 
                context.ViewModel.SearchedQualificationId == context.ViewModel.SelectedQualificationId.Value)
            {
                var remainingCount = context.ViewModel.TotalRecordCount.Value - context.ViewModel.NumberOfItemsToShow.Value;
                
                if (remainingCount >= AppConstants.DefaultNumberOfItemsToShow)
                {
                    context.ViewModel.SelectedItemIndex = context.ViewModel.NumberOfItemsToShow.Value;
                    context.ViewModel.NumberOfItemsToShow = context.ViewModel.NumberOfItemsToShow.Value + AppConstants.DefaultNumberOfItemsToShow;
                }
                else if (remainingCount > 0)
                {
                    context.ViewModel.SelectedItemIndex = context.ViewModel.NumberOfItemsToShow.Value;
                    context.ViewModel.NumberOfItemsToShow = context.ViewModel.TotalRecordCount.Value;
                }
            }

            return Task.CompletedTask;
        }
    }
}

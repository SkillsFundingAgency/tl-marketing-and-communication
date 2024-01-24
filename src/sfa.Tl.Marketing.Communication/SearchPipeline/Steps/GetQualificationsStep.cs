using Microsoft.AspNetCore.Mvc.Rendering;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace sfa.Tl.Marketing.Communication.SearchPipeline.Steps;

public class GetQualificationsStep : ISearchStep
{
    private readonly IProviderSearchService _providerSearchService;

    public GetQualificationsStep(IProviderSearchService providerSearchService)
    {
        _providerSearchService = providerSearchService ?? throw new ArgumentNullException(nameof(providerSearchService));
    }

    public Task Execute(ISearchContext context)
    {
        context.ViewModel.SelectedQualificationId ??= 0;

        var qualifications = _providerSearchService.GetQualifications();

        if (context.ViewModel.SelectedQualificationId != 0 &&
            // ReSharper disable once PossibleMultipleEnumeration
            qualifications.All(q => q.Id != context.ViewModel.SelectedQualificationId))
        {
            context.ViewModel.SelectedQualificationId = 0;
        }

        const int HairdressingBarberingAndBeautyTherapyId = 53;
        const int CateringId = 56;

        var excludedQualifications = new int[] { HairdressingBarberingAndBeautyTherapyId, CateringId };

        // ReSharper disable once PossibleMultipleEnumeration
        context.ViewModel.Qualifications = qualifications
            .Where(q => !excludedQualifications.Contains(q.Id))
            .Select(q =>
                new SelectListItem
                {
                    Text = q.Name,
                    Value = q.Id.ToString(),
                    Selected = q.Id == context.ViewModel.SelectedQualificationId.Value
                });

        return Task.CompletedTask;
    }
}
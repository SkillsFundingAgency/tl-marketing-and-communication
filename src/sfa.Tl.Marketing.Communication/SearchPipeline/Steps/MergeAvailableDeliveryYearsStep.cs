using System;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sfa.Tl.Marketing.Communication.Application.Extensions;
using sfa.Tl.Marketing.Communication.Comparers;

namespace sfa.Tl.Marketing.Communication.SearchPipeline.Steps;

public class MergeAvailableDeliveryYearsStep : ISearchStep
{
    private readonly IDateTimeService _dateTimeService;

    public MergeAvailableDeliveryYearsStep(IDateTimeService dateTimeService)
    {
        _dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
    }

    public Task Execute(ISearchContext context)
    {
        var today = _dateTimeService.Today;

        foreach (var providerLocation in context.ViewModel.ProviderLocations.Where(p =>
                     p.DeliveryYears is not null))
        {
            DeliveryYearViewModel availableNow = null;
            var availableNowToRemove = new List<DeliveryYearViewModel>();

            foreach (var deliveryYear in providerLocation.DeliveryYears)
            {
                deliveryYear.IsAvailableNow = deliveryYear.Year.IsAvailableAtDate(today);

                if (deliveryYear.IsAvailableNow)
                {
                    if (availableNow is null)
                    {
                        availableNow = deliveryYear;
                    }
                    else
                    {
                        var qualificationComparer = new QualificationViewModelComparer();
                        availableNow.Qualifications = availableNow.Qualifications
                            .Union(deliveryYear.Qualifications, qualificationComparer)
                            .OrderBy(q => q.Name)
                            .ToList();

                        availableNowToRemove.Add(deliveryYear);
                    }
                }
            }

            foreach (var deliveryYear in availableNowToRemove)
            {
                providerLocation.DeliveryYears.Remove(deliveryYear);
            }
        }

        return Task.CompletedTask;
    }
}
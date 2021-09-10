using System;
using System.Collections.Generic;
using System.Linq;
using sfa.Tl.Marketing.Communication.Application.Extensions;
using sfa.Tl.Marketing.Communication.Models;

namespace sfa.Tl.Marketing.Communication.Extensions
{
    public static class BusinessRuleExtensions
    {
        public static void MergeAvailableDeliveryYears(
            this IList<ProviderLocationViewModel> providerLocations,
            DateTime today)
        {
            foreach (var providerLocation in providerLocations)
            {
                if (providerLocation.DeliveryYears is null)
                {
                    continue;
                }

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
                                availableNow.Qualifications = availableNow.Qualifications
                                .Union(deliveryYear.Qualifications)
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
        }
    }
}

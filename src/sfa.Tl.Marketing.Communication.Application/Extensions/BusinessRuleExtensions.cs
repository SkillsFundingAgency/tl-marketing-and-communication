using System;
using System.Collections.Generic;
using System.Linq;
using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.Application.Extensions;

public static class BusinessRuleExtensions
{
    public static bool IsAvailableAtDate(this short deliveryYear, DateTime today)
    {
        return deliveryYear < today.Year
               || (deliveryYear == today.Year && today.Month >= 9);
    }
        
    public static IList<Qualification> GetQualificationsForDeliveryYear(
        this DeliveryYearDto deliveryYear,
        IDictionary<int, Qualification> qualificationsDictionary)
    {
        var list = new List<Qualification>();

        if (deliveryYear.Qualifications != null)
        {
            list.AddRange(
                deliveryYear
                    .Qualifications
                    .Select(q => new Qualification
                    {
                        Id = q,
                        Name = qualificationsDictionary[q].Name,
                        Route = qualificationsDictionary[q].Route
                    }));
        }

        return list.OrderBy(q => q.Name).ToList();
    }
}
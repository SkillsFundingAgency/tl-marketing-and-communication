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
        this (DeliveryYearDto DeliveryYear,
              IDictionary<int, Qualification> QualificationsDictionary) 
              p)
    {
        var list = new List<Qualification>();

        if (p.DeliveryYear?.Qualifications != null)
        {
            list.AddRange(
                p.DeliveryYear
                    .Qualifications
                    .Where(q => p.QualificationsDictionary.ContainsKey(q))
                    .Select(q => new Qualification
                    {
                        Id = q,
                        Name = p.QualificationsDictionary[q].Name,
                        Route = p.QualificationsDictionary[q].Route
                    }));
        }

        return list.OrderBy(q => q.Name).ToList();
    }
}
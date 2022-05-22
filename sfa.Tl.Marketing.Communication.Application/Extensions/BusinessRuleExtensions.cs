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
              IDictionary<int, Qualification> QualificationsDictionary) data)
    {
        var list = new List<Qualification>();

        if (data.DeliveryYear?.Qualifications != null)
        {
            list.AddRange(
                data.DeliveryYear
                    .Qualifications
                    .Where(q => data.QualificationsDictionary.ContainsKey(q))
                    .Select(q => new Qualification
                    {
                        Id = q,
                        Name = data.QualificationsDictionary[q].Name,
                        Route = data.QualificationsDictionary[q].Route
                    }));
        }

        return list.OrderBy(q => q.Name).ToList();
    }
}
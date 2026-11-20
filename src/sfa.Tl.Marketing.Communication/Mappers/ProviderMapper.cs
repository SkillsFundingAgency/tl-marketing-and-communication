using System;
using System.Collections.Generic;
using System.Linq;
using Mapster;
using sfa.Tl.Marketing.Communication.Models;
using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.Mappers;

public class ProviderMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Null source collections are mapped to empty lists to preserve the previous AutoMapper behaviour.
        var deliveryYearSetter = config.NewConfig<DeliveryYear, DeliveryYearViewModel>()
            .Map(d => d.Qualifications, s => s.Qualifications ?? new List<Qualification>())
            .Ignore(d => d.IsAvailableNow);
        deliveryYearSetter.Settings.RequireDestinationMemberSource = true;

        var qualificationSetter = config.NewConfig<Qualification, QualificationViewModel>();
        qualificationSetter.Settings.RequireDestinationMemberSource = true;

        var providerLocationSetter = config.NewConfig<ProviderLocation, ProviderLocationViewModel>()
            .Map(d => d.DistanceInMiles, s => (int)Math.Round(s.DistanceInMiles, MidpointRounding.AwayFromZero))
            .Map(d => d.DeliveryYears, s => s.DeliveryYears ?? Enumerable.Empty<DeliveryYear>())
            .Ignore(d => d.HasFocus);
        providerLocationSetter.Settings.RequireDestinationMemberSource = true;
    }
}

using System;
using AutoMapper;
using sfa.Tl.Marketing.Communication.Models;
using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.Mappers
{
    public class ProviderMapper : Profile
    {
        public ProviderMapper()
        {
            CreateMap<DeliveryYear, DeliveryYearViewModel>();

            CreateMap<Qualification, QualificationViewModel>();

            CreateMap<ProviderLocation, ProviderLocationViewModel>()
                .ForMember(m => m.DistanceInMiles, config =>
                    config.MapFrom(s => (int)Math.Floor(s.DistanceInMiles)))
                .ForMember(m => m.HasFocus, config => config.Ignore());
        }
    }
}

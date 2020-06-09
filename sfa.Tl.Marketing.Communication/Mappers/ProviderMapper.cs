using AutoMapper;
using sfa.Tl.Marketing.Communication.Models;
using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.Mappers
{
    public class ProviderMapper : Profile
    {
        public ProviderMapper()
        {
            CreateMap<Qualification, QualificationViewModel>();
            CreateMap<ProviderLocation, ProviderLocationViewModel>();
        }
    }
}

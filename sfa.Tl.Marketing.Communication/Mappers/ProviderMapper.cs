using System;
using System.Collections.Generic;
using System.Text.Json;
using AutoMapper;
using sfa.Tl.Marketing.Communication.Application.Extensions;
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
                    config.MapFrom(s => (int)Math.Round(s.DistanceInMiles, MidpointRounding.AwayFromZero)))
                .ForMember(m => m.HasFocus, config => config.Ignore());
        }

        private static IDictionary<string, VenueNameOverride> GetVenueNameOverrides()
        {
            var venueNameData = JsonSerializer
                .Deserialize<IList<VenueNameOverride>>(
                    "sfa.Tl.Marketing.Communication.Application.Data.VenueNames.json"
                        .ReadManifestResourceStreamAsString(),
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

            var venueNameOverrides = new Dictionary<string, VenueNameOverride>();
            if (venueNameData != null)
            {
                foreach (var venueName in venueNameData)
                {
                    venueNameOverrides[$"{venueName.UkPrn}{venueName.Postcode}"] = venueName;
                }
            }

            return venueNameOverrides;
        }

    }
}

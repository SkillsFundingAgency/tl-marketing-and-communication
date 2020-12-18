using System;
using System.Collections.Generic;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models.Dto;
using System.Linq;

namespace sfa.Tl.Marketing.Communication.Application.Services
{
    public class ProviderLocationService : IProviderLocationService
    {
        private readonly IProviderDataService _providerDataService;

        public ProviderLocationService(IProviderDataService providerDataService)
        {
            _providerDataService = providerDataService;
        }

        public IQueryable<ProviderLocation> GetProviderLocations(IQueryable<Location> locations, IQueryable<Provider> providers)
        {
            return locations.Select(l => new
                {
                    Location = l,
                    Provider = providers.Single(parent => parent.Locations.Contains(l))
                })
                .Select(pl => new ProviderLocation
                {
                    ProviderName = pl.Provider.Name,
                    Name = pl.Location.Name,
                    Latitude = pl.Location.Latitude,
                    Longitude = pl.Location.Longitude,
                    Postcode = pl.Location.Postcode,
                    Town = pl.Location.Town,
                    Website = pl.Location.Website,
                    DeliveryYears = pl.Location.DeliveryYears != null ?
                        pl.Location.DeliveryYears
                        .Select(d => new DeliveryYearWithQualifications
                        {
                            Year = d.Year,
                            Qualifications = d.Qualifications != null ?
                                _providerDataService.GetQualifications(d.Qualifications.ToArray())
                                : new List<Qualification>()
                        })
                        .OrderBy(d => d.Year)
                        .ToList()
                    : new List<DeliveryYearWithQualifications>()
                });
                //Proposed fix for bug TLWP-1284
                //.Where(pl => pl.Qualification2020.Any() || pl.Qualification2021.Any());
        }
    }
}

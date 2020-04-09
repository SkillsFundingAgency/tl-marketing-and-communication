﻿using sfa.Tl.Marketing.Communication.Application.Interfaces;
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
            var providerlocations = locations.Select(l => new { Location = l, Provider = providers.Where(parent => parent.Locations.Contains(l)).Single() })
                        .Select(pl => new ProviderLocation
                        {
                            ProviderName = pl.Provider.Name,
                            Name = pl.Location.Name,
                            Latitude = pl.Location.Latitude,
                            Longitude = pl.Location.Longitude,
                            Postcode = pl.Location.Postcode,
                            Town = pl.Location.Town,
                            Website = pl.Location.Website,
                            Qualification2020 = _providerDataService.GetQualifications(pl.Location.Qualification2020),
                            Qualification2021 = _providerDataService.GetQualifications(pl.Location.Qualification2021),

                        });

            return providerlocations;
        }
    }
}
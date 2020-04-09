using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sfa.Tl.Marketing.Communication.Application.Services
{
    public class LocationService : ILocationService
    {
        public IQueryable<Location> GetLocations(IQueryable<Provider> providers)
        {
            var locations = providers.SelectMany(p => p.Locations);
            return locations;
        }

        public IQueryable<Location> GetLocations(IQueryable<Provider> providers, int qualificationId)
        {
            var locations = providers.SelectMany(p => p.Locations)
                            .Where(l => l.Qualification2020.Contains(qualificationId)
                                        || l.Qualification2021.Contains(qualificationId));

            return locations;
        }
    }
}

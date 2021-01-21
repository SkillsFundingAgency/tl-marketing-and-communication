using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models.Dto;
using System.Linq;

namespace sfa.Tl.Marketing.Communication.Application.Services
{
    public class LocationService : ILocationService
    {
        public IQueryable<Location> GetLocations(IQueryable<Provider> providers, int? qualificationId = null)
        {
            return qualificationId > 0
                ? providers.SelectMany(p => p.Locations)
                    .Where(l => l.DeliveryYears.Any(d => d.Qualifications.Contains(qualificationId.Value)))
                : providers.SelectMany(p => p.Locations);
        }
    }
}

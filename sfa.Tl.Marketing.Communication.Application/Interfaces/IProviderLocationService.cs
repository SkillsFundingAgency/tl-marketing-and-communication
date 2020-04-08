using sfa.Tl.Marketing.Communication.Models.Dto;
using System.Linq;

namespace sfa.Tl.Marketing.Communication.Application.Interfaces
{
    public interface IProviderLocationService
    {
        IQueryable<ProviderLocation> GetProviderLocations(IQueryable<Location> locations);
    }
}

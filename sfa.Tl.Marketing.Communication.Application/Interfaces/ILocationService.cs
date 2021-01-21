using sfa.Tl.Marketing.Communication.Models.Dto;
using System.Linq;

namespace sfa.Tl.Marketing.Communication.Application.Interfaces
{
    public interface ILocationService
    {
        IQueryable<Location> GetLocations(IQueryable<Provider> providers, int? qualificationId = null);
    }
}

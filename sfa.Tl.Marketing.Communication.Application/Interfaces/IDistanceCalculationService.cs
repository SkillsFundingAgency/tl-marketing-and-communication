using sfa.Tl.Marketing.Communication.Models.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sfa.Tl.Marketing.Communication.Application.Interfaces
{
    public interface IDistanceCalculationService
    {
        Task<List<ProviderLocation>> CalculateProviderLocationDistanceInMiles(string originPostcode, IQueryable<ProviderLocation> providerLocations);
        Task<List<ProviderLocation>> CalculateProviderLocationDistanceInMiles(PostcodeLocation origin, IQueryable<ProviderLocation> providerLocations);
        Task<(bool IsValid, PostcodeLocation PostcodeLocation)> IsPostcodeValid(string postcode);
    }
}

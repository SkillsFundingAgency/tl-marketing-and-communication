using sfa.Tl.Marketing.Communication.Models.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sfa.Tl.Marketing.Communication.Application.Interfaces
{
    public interface IDistanceCalculationService
    {
        double CalculateDistanceInMiles(double lat1, double lon1, double lat2, double lon2);

        Task CalculateProviderLocationDistanceInMiles(PostcodeLocation origin, IQueryable<ProviderLocation> providerLocations);
        
        Task<(bool IsValid, PostcodeLocation PostcodeLocation)> IsPostcodeValid(string postcode);
    }
}

using sfa.Tl.Marketing.Communication.Models.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sfa.Tl.Marketing.Communication.Application.Interfaces
{
    public interface IDistanceCalculationService
    {
        Task<List<ProviderLocation>> CalculateProviderLocationDistanceInMiles(string originPostCode, IQueryable<ProviderLocation> providerLocations);
        Task<(bool IsValid, string Postcode)> IsPostcodeValid(string postcode);
    }
}

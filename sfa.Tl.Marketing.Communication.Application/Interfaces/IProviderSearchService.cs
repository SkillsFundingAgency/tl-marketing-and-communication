using sfa.Tl.Marketing.Communication.Models.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sfa.Tl.Marketing.Communication.Application.Interfaces
{
    public interface IProviderSearchService
    {
        Task<(int totalCount, IEnumerable<ProviderLocation> searchResults)> Search(SearchRequest searchRequest);
        IEnumerable<Qualification> GetQualifications();
        IEnumerable<ProviderLocation> GetAllProviderLocations();
        Qualification GetQualificationById(int id);
        Task<(bool IsValid, string Postcode)> IsSearchPostcodeValid(string postcode);
    }
}

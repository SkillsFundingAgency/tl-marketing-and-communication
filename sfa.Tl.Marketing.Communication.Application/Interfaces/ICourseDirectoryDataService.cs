using System.Collections.Generic;
using System.Threading.Tasks;
using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.Application.Interfaces
{
    public interface ICourseDirectoryDataService
    {
        Task<int> ImportProvidersFromCourseDirectoryApi(IList<VenueNameOverride> venueNames);

        Task<int> ImportQualificationsFromCourseDirectoryApi();

        Task<string> GetTLevelDetailJsonFromCourseDirectoryApi();

        Task<string> GetTLevelQualificationJsonFromCourseDirectoryApi();

        Task<IList<Provider>> GetProviders();

        Task<IList<Qualification>> GetQualifications();
    }
}
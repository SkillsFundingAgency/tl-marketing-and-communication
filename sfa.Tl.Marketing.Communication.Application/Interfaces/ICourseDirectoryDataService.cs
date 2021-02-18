using System.Collections.Generic;
using System.Threading.Tasks;
using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.Application.Interfaces
{
    public interface ICourseDirectoryDataService
    {
        Task<(int Saved, int Deleted)> ImportProvidersFromCourseDirectoryApi(IList<VenueNameOverride> venueNames);

        Task<(int Saved, int Deleted)> ImportQualificationsFromCourseDirectoryApi();

        Task<string> GetTLevelDetailJsonFromCourseDirectoryApi();

        Task<string> GetTLevelQualificationJsonFromCourseDirectoryApi();
    }
}
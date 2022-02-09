using System.Threading.Tasks;

namespace sfa.Tl.Marketing.Communication.Application.Interfaces
{
    public interface ICourseDirectoryDataService
    {
        Task<(int Saved, int Deleted)> ImportProvidersFromCourseDirectoryApi();

        Task<(int Saved, int Deleted)> ImportQualificationsFromCourseDirectoryApi();

        Task<string> GetTLevelDetailJsonFromCourseDirectoryApi();

        Task<string> GetTLevelQualificationJsonFromCourseDirectoryApi();
    }
}
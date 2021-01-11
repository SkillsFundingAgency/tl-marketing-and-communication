using System.Threading.Tasks;

namespace sfa.Tl.Marketing.Communication.Data.Interfaces
{
    public interface ICourseDirectoryDataService
    {
        Task<int> ImportFromCourseDirectoryApi();
    }
}
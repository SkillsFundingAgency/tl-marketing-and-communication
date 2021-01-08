using System.Threading.Tasks;

namespace sfa.Tl.Marketing.Communication.Application.Interfaces
{
    public interface ICourseDirectoryDataService
    {
        Task<int> ImportFromCourseDirectoryApi();
    }
}
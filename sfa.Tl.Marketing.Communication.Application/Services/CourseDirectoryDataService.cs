using System.Threading.Tasks;
using sfa.Tl.Marketing.Communication.Application.Interfaces;

namespace sfa.Tl.Marketing.Communication.Application.Services
{
    public class CourseDirectoryDataService : ICourseDirectoryDataService
    {
        public const string CourseDirectoryHttpClientName = "CourseDirectoryAutoCompressClient";

        public async Task<int> ImportFromCourseDirectoryApi()
        {
            return 0;
        }
    }
}
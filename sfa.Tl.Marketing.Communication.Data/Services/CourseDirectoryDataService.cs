using System.Threading.Tasks;
using sfa.Tl.Marketing.Communication.Data.Interfaces;

namespace sfa.Tl.Marketing.Communication.Data.Services
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
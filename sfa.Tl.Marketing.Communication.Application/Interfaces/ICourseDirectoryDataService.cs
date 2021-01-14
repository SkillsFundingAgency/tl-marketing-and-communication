using System.Collections.Generic;
using System.Threading.Tasks;
using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.Application.Interfaces
{
    public interface ICourseDirectoryDataService
    {
        Task<int> ImportFromCourseDirectoryApi();

        Task<IList<Provider>> GetProviders();

        Task<IList<Qualification>> GetQualifications();
    }
}
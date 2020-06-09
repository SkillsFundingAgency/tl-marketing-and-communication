using sfa.Tl.Marketing.Communication.Models;
using System.Threading.Tasks;

namespace sfa.Tl.Marketing.Communication.SearchPipeline
{
    public interface IProviderSearchEngine
    {
        Task<FindViewModel> Search(FindViewModel findViewModel);
    }
}

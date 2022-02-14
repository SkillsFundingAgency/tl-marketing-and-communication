using sfa.Tl.Marketing.Communication.Models;

namespace sfa.Tl.Marketing.Communication.SearchPipeline
{
    public interface ISearchContext
    {
        FindViewModel ViewModel { get; }
        bool Continue { get; set; }
    }
}

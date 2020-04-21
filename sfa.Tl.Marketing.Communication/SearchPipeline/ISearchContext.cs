using sfa.Tl.Marketing.Communication.Models;
using sfa.Tl.Marketing.Communication.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sfa.Tl.Marketing.Communication.SearchPipeline
{
    public interface ISearchContext
    {
        FindViewModel ViewModel { get; }
        bool Continue { get; set; }
    }
}

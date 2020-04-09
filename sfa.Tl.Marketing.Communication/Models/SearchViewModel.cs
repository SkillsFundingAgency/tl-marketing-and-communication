using sfa.Tl.Marketing.Communication.Models.Dto;
using System.Collections.Generic;

namespace sfa.Tl.Marketing.Communication.Models
{
    public class SearchViewModel
    {
        public IEnumerable<ProviderLocationViewModel> ProviderLocations { get; set; }
    }
}
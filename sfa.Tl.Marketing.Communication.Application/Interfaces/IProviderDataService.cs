using sfa.Tl.Marketing.Communication.Models.Dto;
using System.Collections.Generic;
using System.Linq;

namespace sfa.Tl.Marketing.Communication.Application.Interfaces
{
    public interface IProviderDataService
    {
        IQueryable<Provider> GetProviders();
        IEnumerable<Qualification> GetQualifications();
        IEnumerable<Qualification> GetQualifications(int[] qualificationIds);
    }
}

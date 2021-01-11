using System.Collections.Generic;
using System.Threading.Tasks;
using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.Data.Interfaces
{
    public interface ITableStorageService
    {
        Task<int> SaveProviders(IList<Provider> providers);
        Task<IList<Provider>> RetrieveProviders();
        Task<int> SaveQualifications(IList<Qualification> qualifications);
        Task<IList<Qualification>> RetrieveQualifications();
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.Application.Interfaces;

public interface ITableStorageService
{
    Task<int> ClearProviders();
    Task<int> SaveProviders(IList<Provider> providers);
    Task<int> RemoveProviders(IList<Provider> providers);
    Task<IList<Provider>> GetAllProviders();

    Task<int> ClearQualifications();
    Task<int> SaveQualifications(IList<Qualification> qualifications);
    Task<int> RemoveQualifications(IList<Qualification> qualifications);
    Task<IList<Qualification>> GetAllQualifications();

    Task<IList<Town>> GetAllTowns();
}
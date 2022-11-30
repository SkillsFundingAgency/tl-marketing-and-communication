using sfa.Tl.Marketing.Communication.Models.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sfa.Tl.Marketing.Communication.Application.Interfaces;

public interface ITownDataService
{
    Task<int> ImportTowns();

    Task<IEnumerable<Town>> Search(string searchTerm, int maxResults);
}
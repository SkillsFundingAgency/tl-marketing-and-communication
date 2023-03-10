using sfa.Tl.Marketing.Communication.Models.Dto;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace sfa.Tl.Marketing.Communication.Application.Interfaces;

public interface ITownDataService
{
    Task<int> ImportTowns();

    Task<int> ImportTowns(Stream stream);
    
    Task<(bool IsValid, Town Town)> IsSearchTermValid(string searchTerm);

    Task<IEnumerable<Town>> Search(string searchTerm, int maxResults);
}
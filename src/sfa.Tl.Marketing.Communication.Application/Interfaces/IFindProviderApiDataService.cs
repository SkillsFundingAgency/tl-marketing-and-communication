using System.Threading.Tasks;

namespace sfa.Tl.Marketing.Communication.Application.Interfaces;

public interface IFindProviderApiDataService
{
    Task<(int Saved, int Deleted)> ImportProvidersFromFindProviderApi();

    Task<(int Saved, int Deleted)> ImportQualificationsFromFindProviderApi();

    Task<string> GetProvidersJsonFromFindProviderApi();

    Task<string> GetQualificationsJsonFromFindProviderApi();
}
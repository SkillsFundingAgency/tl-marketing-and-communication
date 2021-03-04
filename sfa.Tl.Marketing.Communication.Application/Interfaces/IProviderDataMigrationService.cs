using System.Threading.Tasks;

namespace sfa.Tl.Marketing.Communication.Application.Interfaces
{
    public interface IProviderDataMigrationService
    {
        Task<int> WriteProviders(string qualificationsFilePath);
        Task<int> WriteQualifications(string providersFilePath);
    }
}

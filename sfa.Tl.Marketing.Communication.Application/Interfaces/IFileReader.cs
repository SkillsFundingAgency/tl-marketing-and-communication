using System.Threading.Tasks;

namespace sfa.Tl.Marketing.Communication.Application.Interfaces
{
    public interface IFileReader
    {
        Task<string> ReadAllTextAsync(string filePath);
    }
}

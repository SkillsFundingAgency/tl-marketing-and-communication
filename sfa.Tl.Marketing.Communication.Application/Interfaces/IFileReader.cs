using System.Threading.Tasks;

namespace sfa.Tl.Marketing.Communication.Application.Interfaces
{
    public interface IFileReader
    {
        string ReadAllText(string filePath);
        Task<string> ReadAllTextAsync(string filePath);
    }
}

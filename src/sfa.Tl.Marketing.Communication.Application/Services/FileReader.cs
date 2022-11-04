using sfa.Tl.Marketing.Communication.Application.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace sfa.Tl.Marketing.Communication.Application.Services;

public class FileReader : IFileReader
{
    public async Task<string> ReadAllTextAsync(string filePath)
    {
        return await File.ReadAllTextAsync(filePath);
    }
}
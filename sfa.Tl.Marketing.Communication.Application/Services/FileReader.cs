using sfa.Tl.Marketing.Communication.Application.Interfaces;
using System.IO;

namespace sfa.Tl.Marketing.Communication.Application.Services
{
    public class FileReader : IFileReader
    {
        public string ReadAllText(string filePath)
        {
            return File.ReadAllText(filePath);
        }
    }
}

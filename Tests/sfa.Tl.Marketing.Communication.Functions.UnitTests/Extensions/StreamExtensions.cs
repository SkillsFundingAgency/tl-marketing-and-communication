using System.IO;
using System.Threading.Tasks;

namespace sfa.Tl.Marketing.Communication.Functions.UnitTests.Extensions;

public static class StreamExtensions
{
    public static async Task<string> ReadAsString(this Stream stream)
    {
        stream.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(stream);
        var body = await reader.ReadToEndAsync();
        return body;
    }
}
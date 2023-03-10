using System;
using System.IO;
using System.Reflection;

namespace sfa.Tl.Marketing.Communication.Application.Extensions;

public static class ResourceExtensions
{
    public static Stream ReadManifestResourceStream(this string resourcePath)
    {
        return Assembly
            .GetCallingAssembly()
            .GetManifestResourceStream(resourcePath);
    }

    public static string ReadManifestResourceStreamAsString(this string resourcePath)
    {
        using var stream = Assembly
            .GetCallingAssembly()
            .GetManifestResourceStream(resourcePath);

        if (stream == null)
        {
            throw new Exception($"Stream for '{resourcePath}' not found.");
        }

        using var stringReader = new StreamReader(stream);
        return stringReader.ReadToEnd();
    }
}
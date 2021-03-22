using System;
using Microsoft.Extensions.Configuration;

namespace sfa.Tl.Marketing.Communication.Functions
{
    public static class ConfigurationExtensions
    {
        public static string GetConfigurationValue(this IConfiguration config, string key)
        {
            var value = Environment.GetEnvironmentVariable(key);
            if (string.IsNullOrEmpty(value))
                value = config.GetValue<string>(key);
            if (string.IsNullOrEmpty(value))
                value = config.GetValue<string>($"Values:{key}");

            return value;
        }
    }
}

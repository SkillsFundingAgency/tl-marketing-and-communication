namespace sfa.Tl.Marketing.Communication.Models.Configuration
{
    public static class ConfigurationKeys
    {
        public const string CourseDirectoryApiConfigurationSectionName = "CourseDirectoryApiConfiguration";
        public const string CourseDirectoryApiKeyConfigKey = CourseDirectoryApiConfigurationSectionName + ":ApiKey";
        public const string CourseDirectoryApiBaseUriConfigKey = CourseDirectoryApiConfigurationSectionName + ":ApiBaseUri";

        public const string TableStorageConnectionStringConfigKey = "SharedStorageAccountConnectionString";
    }
}
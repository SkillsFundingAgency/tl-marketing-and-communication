namespace sfa.Tl.Marketing.Communication.Models.Configuration;

public static class ConfigurationKeys
{
    public const string EnvironmentNameConfigKey = "EnvironmentName";
    public const string ConfigurationStorageConnectionStringConfigKey = "ConfigurationStorageConnectionString";
    public const string ServiceNameConfigKey = "ServiceName";
    public const string VersionConfigKey = "Version";
    public const string ServiceVersionConfigKey = "ServiceVersion";

    public const string CacheExpiryInSecondsConfigKey = "CacheExpiryInSeconds";
    public const string PostcodeCacheExpiryInSecondsConfigKey = "PostcodeCacheExpiryInSeconds";
    public const string PostcodeRetrieverBaseUrlConfigKey = "PostcodeRetrieverBaseUrl";

    public const string EmployerSupportSiteUriConfigKey = "EmployerSiteSettings_SiteUrl";
    public const string AboutArticleConfigKey = "EmployerSiteSettings_AboutArticle";
    public const string IndustryPlacementsBenefitsArticleConfigKey = "EmployerSiteSettings_IndustryPlacementsBenefitsArticle";
    public const string SkillsArticleConfigKey = "EmployerSiteSettings_SkillsArticle";
    public const string TimelineArticleConfigKey = "EmployerSiteSettings_TimelineArticle";

    public const string BlobStorageConnectionStringConfigKey = "BlobStorageConnectionString";
    public const string TableStorageConnectionStringConfigKey = "TableStorageConnectionString";
}
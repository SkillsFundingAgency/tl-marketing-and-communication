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

    public const string EmployerSiteSettingsConfigKey = "EmployerSiteSettings";
    public const string EmployerSupportSiteUriConfigKey = "EmployerSiteSettings_SiteUrl";
    public const string EmployerSupportSiteAboutArticleConfigKey = "EmployerSiteSettings_AboutArticle";
    public const string EmployerSupportSiteIndustryPlacementsBenefitsArticleConfigKey = "EmployerSiteSettings_IndustryPlacementsBenefitsArticle";
    public const string EmployerSupportSiteSkillsArticleConfigKey = "EmployerSiteSettings_SkillsArticle";
    public const string EmployerSupportSiteTimelineArticleConfigKey = "EmployerSiteSettings_TimelineArticle";
    
    public static string MergeTempProviderDataConfigKey = "MergeTempProviderData";

    public const string TableStorageConnectionStringConfigKey = "TableStorageConnectionString";
}
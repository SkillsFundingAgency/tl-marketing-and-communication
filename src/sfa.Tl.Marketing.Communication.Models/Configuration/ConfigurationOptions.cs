namespace sfa.Tl.Marketing.Communication.Models.Configuration;

public class ConfigurationOptions
{
    public string Environment { get; init; }
    public EmployerSiteSettings EmployerSiteSettings { get; init; }
    public string PostcodeRetrieverBaseUrl { get; init; }
    public StorageSettings StorageSettings { get; init; }
    public int CacheExpiryInSeconds { get; init; }
    public CourseDirectoryApiSettings CourseDirectoryApiSettings { get; init; }
    public int PostcodeCacheExpiryInSeconds { get; init; }
    public bool MergeTempProviderData { get; init; }
}
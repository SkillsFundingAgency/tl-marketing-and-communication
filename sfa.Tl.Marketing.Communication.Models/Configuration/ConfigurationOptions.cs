namespace sfa.Tl.Marketing.Communication.Models.Configuration;

public class ConfigurationOptions
{
    public string EmployerSupportSiteUrl { get; init; }
    public string PostcodeRetrieverBaseUrl { get; init; }
    public StorageSettings StorageConfiguration { get; init; }
    public int CacheExpiryInSeconds { get; init; }
    public int PostcodeCacheExpiryInSeconds { get; init; }
    public bool MergeTempProviderData { get; init; }
}
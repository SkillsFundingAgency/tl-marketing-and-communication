namespace sfa.Tl.Marketing.Communication.Models.Configuration
{
    public class ConfigurationOptions
    {
        public string EmployerSupportSiteUrl { get; set; }
        public string PostcodeRetrieverBaseUrl { get; set; }
        public StorageSettings StorageConfiguration { get; set; }
        public int CacheExpiryInSeconds { get; set; }
    }
}

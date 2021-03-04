namespace sfa.Tl.Marketing.Communication.Models.Configuration
{
    public class ConfigurationOptions
    {
        public string ProvidersDataFilePath { get; set; }
        public string QualificationsDataFilePath { get; set; }
        public string EmployerContactEmailTemplateId { get; set; }
        public string SupportEmailInboxAddress { get; set; }
        public string PostcodeRetrieverBaseUrl { get; set; }
        public StorageSettings StorageConfiguration { get; set; }
        public int CacheExpiryInSeconds { get; set; }
    }
}

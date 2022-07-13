using System;
using sfa.Tl.Marketing.Communication.Models.Configuration;

// ReSharper disable UnusedMember.Global

namespace sfa.Tl.Marketing.Communication.UnitTests.Builders;

// ReSharper disable once ClassNeverInstantiated.Global
internal class SettingsBuilder
{
    private const string FindCourseApiKey = "0f608e5d437f4baabc04a0bc2dabbc1b";
    private const string FindCourseApiBaseAbsoluteUri = "https://test.com/findacourse/api";
    private const string FindProviderApiBaseAbsoluteUri = "https://test.com/findaprovider/api";
    private const string PostcodeRetrieverAbsoluteUri = "https://test.postcodes.com/";
    private const string EmployerSiteUri = "https://test.employers.gov.uk/";
    private const string EmployerAboutArticle = "categories/12345-About";
    private const string FindProviderAppId = "8BA77340-32B0-4E0D-B149-A24DF46434F2";
    private const string FindProviderApiKey = "ACD256BE-1180-4EAE-B4EC-124225691D94";

    public static readonly Uri FindCourseApiBaseUri = new(FindCourseApiBaseAbsoluteUri);
    public static readonly Uri FindProviderApiBaseUri = new(FindProviderApiBaseAbsoluteUri);
    public static readonly Uri PostcodeRetrieverBaseUri = new(PostcodeRetrieverAbsoluteUri);

    internal CourseDirectoryApiSettings BuildCourseDirectoryApiSettings(
        string findCourseApiBaseUri = FindCourseApiBaseAbsoluteUri,
        string findCourseApiKey = FindCourseApiKey) => new()
    {
        BaseUri = findCourseApiBaseUri,
        ApiKey = findCourseApiKey
    };

    internal FindProviderApiSettings BuildFindProviderApiSettings(
        string findProviderApiBaseUri = FindProviderApiBaseAbsoluteUri,
        string findProviderAppId = FindProviderAppId,
        string findProviderApiKey = FindProviderApiKey) => new ()
    {
        BaseUri = findProviderApiBaseUri,
        AppId = findProviderApiKey,
        ApiKey = findProviderApiKey
    };

    internal EmployerSiteSettings BuildEmployerSiteSettings(
        string employerSiteUri = EmployerSiteUri,
        string employerAboutArticle = EmployerAboutArticle) => new()
    {
       SiteUrl = employerSiteUri,
        AboutArticle = employerAboutArticle
    };

    internal StorageSettings BuildStorageSettings(
        string tableStorageConnectionString = "TestConnection") => new()
    {
        TableStorageConnectionString = tableStorageConnectionString
    };
}
using System;
using sfa.Tl.Marketing.Communication.Models.Configuration;

// ReSharper disable UnusedMember.Global

namespace sfa.Tl.Marketing.Communication.UnitTests.Builders;

// ReSharper disable once ClassNeverInstantiated.Global
internal class SettingsBuilder
{
    private const string FindCourseApiKey = "0f608e5d437f4baabc04a0bc2dabbc1b";
    private const string FindCourseApiBaseAbsoluteUri = "https://test.com/findacourse/api";
    private const string PostcodeRetrieverAbsoluteUri = "https://test.postcodes.com/";

    public static readonly Uri FindCourseApiBaseUri = new(FindCourseApiBaseAbsoluteUri);
    public static readonly Uri PostcodeRetrieverBaseUri = new(PostcodeRetrieverAbsoluteUri);

    internal CourseDirectoryApiSettings BuildApiSettings(
        string findCourseApiBaseUri = FindCourseApiBaseAbsoluteUri,
        string findCourseApiKey = FindCourseApiKey) => new()
    {
        ApiBaseUri = findCourseApiBaseUri,
        ApiKey = findCourseApiKey
    };

    internal StorageSettings BuildStorageSettings(
        string tableStorageConnectionString = "TestConnection") => new()
    {
        TableStorageConnectionString = tableStorageConnectionString
    };
}
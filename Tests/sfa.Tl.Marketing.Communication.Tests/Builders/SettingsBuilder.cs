using System;
using sfa.Tl.Marketing.Communication.Models.Configuration;

// ReSharper disable UnusedMember.Global

namespace sfa.Tl.Marketing.Communication.UnitTests.Builders
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class SettingsBuilder
    {
        public const string FindCourseApiBaseAbsoluteUri = "https://test.com/findacourse/api";
        public static readonly Uri FindCourseApiBaseUri = new(FindCourseApiBaseAbsoluteUri);
        public const string FindCourseApiKey = "0f608e5d437f4baabc04a0bc2dabbc1b";

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
}

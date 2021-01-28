using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Application.Repositories;
using sfa.Tl.Marketing.Communication.Application.Services;
using sfa.Tl.Marketing.Communication.DataLoad.PostcodesIo;
using sfa.Tl.Marketing.Communication.DataLoad.Read;
using sfa.Tl.Marketing.Communication.DataLoad.Write;
using sfa.Tl.Marketing.Communication.Models.Entities;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace sfa.Tl.Marketing.Communication.DataLoad
{
    class Program
    {
        private const string CsvFilePath = @"D:\SFA-TestData\Full Provider Data 2020 - 2021 (campaign site).csv";
        private const string JsonOutputPath = @"D:\SFA-TestData\Json\providers.json";
        private const string PostcodesIoUrl = "https://postcodes.io";


        private static IList<string> _warningMessages;

        // ReSharper disable once UnusedParameter.Local
        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true);

            var configuration = builder.Build();

            var inputFilePath = configuration.GetValue<string>("InputFilePath");
            if (string.IsNullOrWhiteSpace(inputFilePath))
                inputFilePath = CsvFilePath;

            var outputFilePath = configuration.GetValue<string>("OutputFilePath");
            if (string.IsNullOrWhiteSpace(outputFilePath))
                outputFilePath = JsonOutputPath;

            var tableStorageConnectionString = configuration.GetValue<string>("TableStorageConnectionString");
            if (!string.IsNullOrEmpty(tableStorageConnectionString))
            {
                var jsonInputOnly = configuration.GetValue<bool>("JsonInputOnly");

                var loggerFactory = new LoggerFactory();
                var providerDataMigrationService = new ProviderDataMigrationService(
                    new FileReader(),
                    CreateTableStorageService(tableStorageConnectionString, loggerFactory),
                    loggerFactory.CreateLogger<ProviderDataMigrationService>());

                var qualificationsSaved = await providerDataMigrationService
                    .WriteQualifications(configuration.GetValue<string>("QualificationJsonInputFilePath"));
                Console.WriteLine("");
                Console.WriteLine($"Copied {qualificationsSaved} qualifications to table storage.");

                var providersSaved = await providerDataMigrationService
                    .WriteProviders(configuration.GetValue<string>("ProviderJsonInputFilePath"));
                Console.WriteLine($"Copied {providersSaved} providers to table storage.");

                if (jsonInputOnly) return;
            }

            var providerReader = new ProviderReader();
            var providerLoadResult = providerReader.ReadData(inputFilePath);

            var providerWrite = new ProviderWrite();
            var providerWriteData = new List<ProviderWriteData>();
            var index = 0;

            var groupedProviders = providerLoadResult.Providers.GroupBy(p => p.ProviderName.Trim());

            _warningMessages = new List<string>();

            foreach (var provider in groupedProviders)
            {
                index++;

                Console.WriteLine($"Processing provider {index} {provider.Key}");

                var writeData = new ProviderWriteData
                {
                    Id = index,
                    Name = provider.Key, //.ToTitleCase(), //Force to title case
                    Locations = GetLocationsWrite(provider)
                };
                providerWriteData.Add(writeData);
            }

            providerWrite.Providers = providerWriteData;

            Console.WriteLine("");
            Console.WriteLine(
                $"Processed {providerWrite.Providers.Count} providers. {_warningMessages.Count} warnings.");
            Console.WriteLine($"Saving providers to {outputFilePath}");

            await WriteProvidersToFile(providerWrite, outputFilePath);
        }

        private static List<LocationWriteData> GetLocationsWrite(IGrouping<string, ProviderReadData> providers)
        {
            var locationWriteData = new List<LocationWriteData>();

            var groupedProviderVenues = providers
                .GroupBy(p => p.Postcode.Trim());

            foreach (var venueGroup in groupedProviderVenues)
            {
                var venue = venueGroup.First();
                var postcode = venue.Postcode.Trim();

                (string FormattedPostcode, double Lat, double Long) postcodeDetails
                    = GetPostcodeDetails(postcode);

                var location = new LocationWriteData
                {
                    Name = venue.VenueName.Trim(),
                    Postcode = postcodeDetails.FormattedPostcode,
                    Town = venue.Town.Trim(),
                    Latitude = postcodeDetails.Lat,
                    Longitude = postcodeDetails.Long,
                    Website = venue.Website.Trim(),
                    DeliveryYears = GetDeliveryYears(venueGroup)
                };

                var logMessage = new StringBuilder($"  Adding location {postcode} " +
                                                   $"{(string.IsNullOrWhiteSpace(venue.VenueName) ? "" : venue.VenueName + ' ')}");
                if (location.DeliveryYears.Any())
                {
                    for (var i = 0; i < location.DeliveryYears.Count; i++)
                    {
                        logMessage.Append(i == 0 ? "with " : "and ");
                        logMessage.Append(
                            $"{location.DeliveryYears[i].Qualifications.Count} {location.DeliveryYears[i].Year} ");
                    }

                    logMessage.Append("qualifications");
                }

                Console.WriteLine(logMessage);

                locationWriteData.Add(location);
            }

            return locationWriteData;
        }

        private static (string, double, double) GetPostcodeDetails(string postcode)
        {
            var postcodesIoUrl = $"{PostcodesIoUrl}/postcodes/{postcode}";
            var httpClient = new HttpClient();
            var responseMessage = httpClient.GetAsync(postcodesIoUrl).GetAwaiter().GetResult();
            if (responseMessage.StatusCode != HttpStatusCode.NotFound)
            {
                responseMessage.EnsureSuccessStatusCode();

                var stream = responseMessage.Content.ReadAsStreamAsync()
                    .GetAwaiter().GetResult();
                var response = JsonSerializer
                    .DeserializeAsync<PostcodeLookupResponse>(stream)
                    .GetAwaiter().GetResult();

                // ReSharper disable once PossibleNullReferenceException
                return (response.Result.Postcode,
                    response.Result.Latitude,
                    response.Result.Longitude);
            }
            else
            {
                postcodesIoUrl = $"{PostcodesIoUrl}/terminated_postcodes/{postcode}";
                var terminatedResponseMessage = httpClient.GetAsync(postcodesIoUrl).GetAwaiter().GetResult();
                if (terminatedResponseMessage.StatusCode != HttpStatusCode.NotFound)
                {
                    terminatedResponseMessage.EnsureSuccessStatusCode();

                    var stream = terminatedResponseMessage.Content.ReadAsStreamAsync()
                        .GetAwaiter().GetResult();
                    var response = JsonSerializer
                        .DeserializeAsync<PostcodeLookupResponse>(stream)
                        .GetAwaiter().GetResult();

                    // ReSharper disable once PossibleNullReferenceException
                    return (response.Result.Postcode,
                        response.Result.Latitude,
                        response.Result.Longitude);
                }
                else
                {
                    throw new Exception($"Location cannot be found {postcode}");
                }
            }
        }

        private static void AddQualification(IList<int> qualificationList, Type qualificationType, int year,
            ProviderReadData venue)
        {
            if (qualificationList.Contains((int)qualificationType))
            {
                var message =
                    $"Warning: Duplicate qualification {qualificationType} for provider {venue.ProviderName} postcode {venue.Postcode} year {year}";
                _warningMessages.Add(message);

                var originalColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(message);
                Console.ForegroundColor = originalColor;
            }
            else
            {
                qualificationList.Add((int)qualificationType);
            }
        }

        private static List<DeliveryYearWriteData> GetDeliveryYears(IGrouping<string, ProviderReadData> venueGroup)
        {
            var deliveryYears = new List<DeliveryYearWriteData>();

            foreach (var venue in venueGroup)
            {
                if (short.TryParse(venue.CourseYear, out var year))
                {
                    var deliveryYear = deliveryYears.FirstOrDefault(d => d.Year == year);
                    if (deliveryYear == null)
                    {
                        deliveryYear = new DeliveryYearWriteData
                        {
                            Year = year,
                            Qualifications = new List<int>()
                        };

                        deliveryYears.Add(deliveryYear);
                    }

                    if (venue.IsDigitalProduction)
                        AddQualification(deliveryYear.Qualifications, Type.DigitalProductionDesignDevelopment, year,
                            venue);
                    else if (venue.IsDigitalBusiness)
                        AddQualification(deliveryYear.Qualifications, Type.DigitalBusiness, year, venue);
                    else if (venue.IsDigitalSupport)
                        AddQualification(deliveryYear.Qualifications, Type.DigitalSupportServices, year, venue);
                    else if (venue.IsDesign)
                        AddQualification(deliveryYear.Qualifications, Type.DesignSurveyingPlanning, year, venue);
                    else if (venue.IsBuildingServices)
                        AddQualification(deliveryYear.Qualifications, Type.BuildingServicesEngineering, year, venue);
                    else if (venue.IsConstruction)
                        AddQualification(deliveryYear.Qualifications, Type.OnsiteConstruction, year, venue);
                    else if (venue.IsEducation)
                        AddQualification(deliveryYear.Qualifications, Type.Education, year, venue);
                    else if (venue.IsHealth)
                        AddQualification(deliveryYear.Qualifications, Type.Health, year, venue);
                    else if (venue.IsHealthCare)
                        AddQualification(deliveryYear.Qualifications, Type.HealthCareScience, year, venue);
                    else if (venue.IsScience)
                        AddQualification(deliveryYear.Qualifications, Type.Science, year, venue);
                }
            }

            return deliveryYears;
        }

        private static async Task WriteProvidersToFile(ProviderWrite data, string path)
        {
            await using var fs = File.Create(path);

            var serializerOptions = new JsonSerializerOptions
            {
                //Use relaxed encoder to allow '&' through
                //https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-how-to#serialize-all-characters
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            await JsonSerializer.SerializeAsync(fs, data, serializerOptions);
        }

        private static ITableStorageService CreateTableStorageService(
            string tableStorageConnectionString,
            ILoggerFactory loggerFactory)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(tableStorageConnectionString);

            var cloudTableClient = cloudStorageAccount.CreateCloudTableClient();

            ICloudTableRepository<ProviderEntity> providerRepository = new GenericCloudTableRepository<ProviderEntity, int>(
                cloudTableClient,
                loggerFactory.CreateLogger<GenericCloudTableRepository<ProviderEntity, int>>());

            ICloudTableRepository<QualificationEntity> qualificationRepository =
                    new GenericCloudTableRepository<QualificationEntity, int>(
                        cloudTableClient,
                        loggerFactory.CreateLogger<GenericCloudTableRepository<QualificationEntity, int>>());

            return new TableStorageService(
            providerRepository,
            qualificationRepository,
            loggerFactory.CreateLogger<TableStorageService>());
        }
    }
}
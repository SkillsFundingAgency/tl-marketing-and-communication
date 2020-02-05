﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using sfa.Tl.Marketing.Communication.DataLoad.PostcodesIo;
using sfa.Tl.Marketing.Communication.DataLoad.Read;
using sfa.Tl.Marketing.Communication.DataLoad.Write;

namespace sfa.Tl.Marketing.Communication.DataLoad
{
    class Program
    {
        private const string CsvFilePath = @"C:\users\nanda\documents\Full Provider Data 2020 - 2021.csv";
        private const string JsonOutputPath = @"C:\Dev\Esfa\T Level Provider Mapping Survey 2020 final071019 -- EIDTED.json";
        private const string PostcodesIoUrl = "https://postcodes.io";

        // ReSharper disable once UnusedParameter.Local
        static void Main(string[] args)
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

            var providerReader = new ProviderReader();
            var providerLoadResult = providerReader.ReadData(inputFilePath);

            var providerWrite = new ProviderWrite();
            var providerWriteData = new List<ProviderWriteData>();
            var index = 0;

            var groupedProviders = providerLoadResult.Providers.GroupBy(p => p.ProviderName.Trim());

            foreach (var provider in groupedProviders)
            {
                index++;

                Console.WriteLine($"Processing provider {index} {provider.Key}");

                var writeData = new ProviderWriteData
                {
                    Id = index,
                    Name = provider.Key,
                    Locations = GetLocationsWrite(provider)
                };
                providerWriteData.Add(writeData);
            }

            providerWrite.Providers = providerWriteData;

            WriteProvidersToFile(providerWrite, outputFilePath);
        }

        private static List<LocationWriteData> GetLocationsWrite(IGrouping<string, ProviderReadData> providers)
        {
            var locationWriteData = new List<LocationWriteData>();

            var groupedProviderVenues = providers
                .GroupBy(p => p.VenueName.Trim());
            foreach (var venueGroup in groupedProviderVenues)
            {
                var venue = venueGroup.First();
                var postcode = venue.Postcode.Trim();
                var latLong = GetLatLong(postcode);

                Console.WriteLine($"  Adding location {venue.VenueName} {postcode}");

                var location = new LocationWriteData
                {
                    Name = venue.VenueName.Trim(),
                    Postcode = postcode,
                    Town = venue.Town.Trim(),
                    Latitude = latLong.Item1,
                    Longitude = latLong.Item2,
                    Website = venue.Website.Trim(),
                    Qualification2020 = new int[0],
                    Qualification2021 = new int[0]
                };

                location.Qualification2020 = GetQualifications(venueGroup, 2020);
                location.Qualification2021 = GetQualifications(venueGroup, 2021);

                locationWriteData.Add(location);
            }

            return locationWriteData;
        }

        private static Tuple<double, double> GetLatLong(string postcode)
        {
            var postcodesIoUrl = $"{PostcodesIoUrl}/postcodes/{postcode}";
            var httpClient = new HttpClient();
            var responseMessage = httpClient.GetAsync(postcodesIoUrl).GetAwaiter().GetResult();
            if (responseMessage.StatusCode != HttpStatusCode.NotFound)
            {
                responseMessage.EnsureSuccessStatusCode();
                var response = responseMessage.Content.ReadAsAsync<PostCodeLookupResponse>().GetAwaiter()
                    .GetResult();

                return new Tuple<double, double>(
                    Convert.ToDouble(response.result.Latitude),
                    Convert.ToDouble(response.result.Longitude));
            }
            else
            {
                postcodesIoUrl = $"{PostcodesIoUrl}/terminated_postcodes/{postcode}";
                var terminatedResponseMessage = httpClient.GetAsync(postcodesIoUrl).GetAwaiter().GetResult();
                if (terminatedResponseMessage.StatusCode != HttpStatusCode.NotFound)
                {
                    terminatedResponseMessage.EnsureSuccessStatusCode();
                    var response = terminatedResponseMessage.Content.ReadAsAsync<PostCodeLookupResponse>().GetAwaiter()
                        .GetResult();

                    return new Tuple<double, double>(
                        Convert.ToDouble(response.result.Latitude),
                        Convert.ToDouble(response.result.Longitude));
                }
                else
                {
                    throw new Exception($"Location cannot be found {postcode}");
                }
            }
        }

        private static int[] GetQualifications(IGrouping<string, ProviderReadData> venueGroup, int year)
        {
            var qualifications = new List<int>();

            foreach (var venue in venueGroup)
            {
                if (venue.CourseYear == year.ToString())
                {
                    if (venue.IsDigitalProduction)
                        qualifications.Add((int)Type.DigitalProductionDesignDevelopment);
                    else if (venue.IsDigitalBusiness)
                        qualifications.Add((int)Type.DigitalBusiness);
                    else if (venue.IsDigitalSupport)
                        qualifications.Add((int)Type.DigitalSupportServices);
                    else if (venue.IsDesign)
                        qualifications.Add((int)Type.DesignSurveyingPlanning);
                    else if (venue.IsBuildingServices)
                        qualifications.Add((int)Type.BuildingServicesEngineering);
                    else if (venue.IsConstruction)
                        qualifications.Add((int)Type.OnsiteConstruction);
                    else if (venue.IsEducation)
                        qualifications.Add((int)Type.Education);
                    else if (venue.IsHealth)
                        qualifications.Add((int)Type.Health);
                    else if (venue.IsHealthCare)
                        qualifications.Add((int)Type.HealthCareScience);
                    else if (venue.IsScience)
                        qualifications.Add((int)Type.Science);
                }
            }

            return qualifications.ToArray();
        }

        private static void WriteProvidersToFile(ProviderWrite data, string path)
        {
            using (var fs = File.Open(path, FileMode.OpenOrCreate))
            using (var sw = new StreamWriter(fs))
            using (var jw = new JsonTextWriter(sw))
            {
                var serializer = new JsonSerializer
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Formatting = Formatting.Indented
                };

                serializer.Serialize(jw, data);
            }
        }

    }
}
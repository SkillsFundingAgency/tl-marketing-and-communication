using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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

        static void Main(string[] args)
        {
            var providerReader = new ProviderReader();
            var providerLoadResult = providerReader.ReadData(CsvFilePath);

            var providerWrite = new ProviderWrite();
            var providerWriteData = new List<ProviderWriteData>();
            var index = 0;

            var groupedProviders = providerLoadResult.Providers.GroupBy(p => p.ProviderName.Trim());

            foreach (var provider in groupedProviders)
            {
                index++;
                var writeData = new ProviderWriteData
                {
                    Id = index,
                    Name = provider.Key,
                    //Website = provider.Website.Trim(),
                    // TODO Work out what we're doing with locations
                    Locations = GetLocationsWrite(provider)
                };
                providerWriteData.Add(writeData);
            }

            providerWrite.Providers = providerWriteData;

            var jsonString = JsonConvert.SerializeObject(providerWrite, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            });

            WriteData(jsonString);
        }

        private static List<LocationWriteData> GetLocationsWrite(IGrouping<string, ProviderReadData> providers)
        {
            var locationWriteData = new List<LocationWriteData>();

            foreach (var p in providers)
            {
                var location = new LocationWriteData
                {
                    Postcode = p.Postcode.Trim(),
                    Town = p.Town.Trim(),
                    Website = p.Website.Trim(),
                    Qualification2020 = GetQualifications2020(p),
                    Qualification2021 = GetQualifications2021(p)
                };

                var postcodesIoUrl = $"{PostcodesIoUrl}/postcodes/{location.Postcode}";
                var httpClient = new HttpClient();
                var responseMessage = httpClient.GetAsync(postcodesIoUrl).GetAwaiter().GetResult();
                if (responseMessage.StatusCode != HttpStatusCode.NotFound)
                {
                    responseMessage.EnsureSuccessStatusCode();
                    var response = responseMessage.Content.ReadAsAsync<PostCodeLookupResponse>().GetAwaiter()
                        .GetResult();

                    location.Latitude = Convert.ToDouble(response.result.Latitude);
                    location.Longitude = Convert.ToDouble(response.result.Longitude);

                    locationWriteData.Add(location);
                }
                else
                {
                    postcodesIoUrl = $"{PostcodesIoUrl}/terminated_postcodes/{location.Postcode}";
                    var terminatedResponseMessage = httpClient.GetAsync(postcodesIoUrl).GetAwaiter().GetResult();
                    if (terminatedResponseMessage.StatusCode != HttpStatusCode.NotFound)
                    {
                        terminatedResponseMessage.EnsureSuccessStatusCode();
                        var response = terminatedResponseMessage.Content.ReadAsAsync<PostCodeLookupResponse>().GetAwaiter()
                            .GetResult();

                        location.Latitude = Convert.ToDouble(response.result.Latitude);
                        location.Longitude = Convert.ToDouble(response.result.Longitude);

                        locationWriteData.Add(location);
                    }
                    else
                    {
                        throw new Exception($"Location cannot be found {location.Postcode}");
                    }
                }

            }

            return locationWriteData;
        }

        private static int[] GetQualifications2021(ProviderReadData providerReadData)
        {
            var qualifications2021 = new List<int>();

            if (providerReadData.IsDigitalProduction && providerReadData.CourseYear == "2021")
                qualifications2021.Add((int)Type.DigitalProductionDesignDevelopment);

            if (providerReadData.IsDigitalBusiness && providerReadData.CourseYear == "2021")
                qualifications2021.Add((int)Type.DigitalBusiness);

            if (providerReadData.IsDigitalSupport && providerReadData.CourseYear == "2021")
                qualifications2021.Add((int)Type.DigitalSupportServices);

            if (providerReadData.IsDesign && providerReadData.CourseYear == "2021")
                qualifications2021.Add((int)Type.DesignSurveyingPlanning);

            if (providerReadData.IsBuildingServices && providerReadData.CourseYear == "2021")
                qualifications2021.Add((int)Type.BuildingServicesEngineering);

            if (providerReadData.IsConstruction && providerReadData.CourseYear == "2021")
                qualifications2021.Add((int)Type.OnsiteConstruction);

            if (providerReadData.IsEducation && providerReadData.CourseYear == "2021")
                qualifications2021.Add((int)Type.Education);

            if (providerReadData.IsHealth && providerReadData.CourseYear == "2021")
                qualifications2021.Add((int)Type.Health);

            if (providerReadData.IsHealthCare && providerReadData.CourseYear == "2021")
                qualifications2021.Add((int)Type.HealthCareScience);

            if (providerReadData.IsScience && providerReadData.CourseYear == "2021")
                qualifications2021.Add((int)Type.Science);


            return qualifications2021.ToArray();
        }

        private static int[] GetQualifications2020(ProviderReadData providerReadData)
        {
            var qualifications2020 = new List<int>();

            if (providerReadData.IsDigitalProduction && providerReadData.CourseYear == "2020")
                qualifications2020.Add((int)Type.DigitalProductionDesignDevelopment);

            if (providerReadData.IsDigitalBusiness && providerReadData.CourseYear == "2020")
                qualifications2020.Add((int)Type.DigitalBusiness);

            if (providerReadData.IsDigitalSupport && providerReadData.CourseYear == "2020")
                qualifications2020.Add((int)Type.DigitalSupportServices);

            if (providerReadData.IsDesign && providerReadData.CourseYear == "2020")
                qualifications2020.Add((int)Type.DesignSurveyingPlanning);

            if (providerReadData.IsBuildingServices && providerReadData.CourseYear == "2020")
                qualifications2020.Add((int)Type.BuildingServicesEngineering);

            if (providerReadData.IsConstruction && providerReadData.CourseYear == "2020")
                qualifications2020.Add((int)Type.OnsiteConstruction);

            if (providerReadData.IsEducation && providerReadData.CourseYear == "2020")
                qualifications2020.Add((int)Type.Education);

            if (providerReadData.IsHealth && providerReadData.CourseYear == "2020")
                qualifications2020.Add((int)Type.Health);

            if (providerReadData.IsHealthCare && providerReadData.CourseYear == "2020")
                qualifications2020.Add((int)Type.HealthCareScience);

            if (providerReadData.IsScience && providerReadData.CourseYear == "2020")
                qualifications2020.Add((int)Type.Science);


            return qualifications2020.ToArray();
        }

        private static void WriteData(string jsonString)
        {
            using (var file = new StreamWriter(JsonOutputPath, false))
                file.WriteLine(jsonString);
        }
    }
}
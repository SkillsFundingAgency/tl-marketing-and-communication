using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models.Configuration;
using sfa.Tl.Marketing.Communication.Models.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace sfa.Tl.Marketing.Communication.Application.Services
{
    public class ProviderDataService : IProviderDataService
    {
        private readonly IFileReader _fileReader;
        private readonly ConfigurationOptions _configurationOptions;
        private readonly JsonDocument _providersData;
        private readonly JsonDocument _qualificationsData;

        public ProviderDataService(IFileReader fileReader, ConfigurationOptions configurationOptions)
        {
            _fileReader = fileReader;
            _configurationOptions = configurationOptions;
            _providersData = GetProvidersData();
            _qualificationsData = GetQualificationsData();
        }

        public IQueryable<Provider> GetProviders()
        {
            var providers = GetAllProviders();
            return providers;
        }

        public IEnumerable<Qualification> GetQualifications(int[] qualificationIds)
        {
            var qualifications = GetAllQualifications();
            return qualifications.Where(q => qualificationIds.Contains(q.Id));
        }

        public Qualification GetQualification(int qualificationId)
        {
            var qualifications = GetAllQualifications();
            return qualifications.SingleOrDefault(q => q.Id == qualificationId);
        }

        public IEnumerable<Qualification> GetQualifications()
        {
            var qualifications = GetAllQualifications().ToList();
            qualifications.Add(new Qualification { Id = 0, Name = "All T Level courses" });
            return qualifications;
        }

        private JsonDocument GetProvidersData()
        {
            var json = _fileReader.ReadAllText(_configurationOptions.ProvidersDataFilePath);
            var jsonDoc = JsonDocument.Parse(json);
            return jsonDoc;
        }

        private JsonDocument GetQualificationsData()
        {
            var json = _fileReader.ReadAllText(_configurationOptions.QualificationsDataFilePath);
            var jsonDoc = JsonDocument.Parse(json);
            return jsonDoc;
        }
        
        public IEnumerable<string> GetWebsiteUrls()
        {
            var urlList = new List<string>();

            foreach (var provider in GetAllProviders())
            {
                foreach (var location in provider.Locations.Where(l => !string.IsNullOrWhiteSpace(l.Website)))
                {
                    if (!urlList.Contains(location.Website))
                    {
                        urlList.Add(location.Website);
                    }
                }
            }

            return urlList;
        }

        private IQueryable<Qualification> GetAllQualifications()
        {
            return _qualificationsData
                .RootElement
                .GetProperty("qualifications")
                .EnumerateObject()
                .Select(q =>
                    new Qualification
                    {
                        Id = int.Parse(q.Name),
                        Name = q.Value.GetString()
                    })
                .AsQueryable();
        }

        private IQueryable<Provider> GetAllProviders()
        {
            var providers = _providersData
                .RootElement
                .GetProperty("providers")
                .EnumerateArray()
                .Select(p =>
                    new Provider
                    {
                        Id = p.GetProperty("id").GetInt32(),
                        Name = p.GetProperty("name").GetString(),
                        Locations = p.GetProperty("locations")
                            .EnumerateArray()
                            .Select(l =>
                                new Location
                                {
                                    Postcode = l.GetProperty("postcode").GetString(),
                                    Name = l.GetProperty("name").GetString(),
                                    Town = l.GetProperty("town").GetString(),
                                    Latitude = l.GetProperty("latitude").GetDouble(),
                                    Longitude = l.GetProperty("longitude").GetDouble(),
                                    Website = l.GetProperty("website").GetString(),
                                    DeliveryYears = l.TryGetProperty("deliveryYears", out var deliveryYears)
                                        ? deliveryYears.EnumerateArray()
                                            .Select(d =>
                                                new DeliveryYear
                                                { 
                                                    Year = d.GetProperty("year").GetInt16(),
                                                    Qualifications = d.GetProperty("qualifications")
                                                        .EnumerateArray()
                                                        .Select(q => q.GetInt32())
                                                        .ToList()
                                                })
                                            .ToList()
                                        : new List<DeliveryYear>()
                                }).ToList()
                    })
                .ToList();

            return providers.AsQueryable();
        }
    }
}

using Newtonsoft.Json.Linq;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models.Configuration;
using sfa.Tl.Marketing.Communication.Models.Dto;
using System.Collections.Generic;
using System.Linq;

namespace sfa.Tl.Marketing.Communication.Application.Services
{
    public class ProviderDataService : IProviderDataService
    {
        private readonly IFileReader _fileReader;
        private readonly IJsonConvertor _jsonConvertor;
        private readonly ConfigurationOptions _configurationOptions;
        private readonly JObject _providersData;

        public ProviderDataService(IFileReader fileReader, IJsonConvertor jsonConvertor, ConfigurationOptions configurationOptions)
        {
            _fileReader = fileReader;
            _jsonConvertor = jsonConvertor;
            _configurationOptions = configurationOptions;
            _providersData = GetProvidersData();
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

        private JObject GetProvidersData()
        {
            var json = _fileReader.ReadAllText(_configurationOptions.DataFilePath);
            var providersDataObject = _jsonConvertor.DeserializeObject<JObject> (json);
            return providersDataObject;
        }

        private IQueryable<Qualification> GetAllQualifications()
        {
            var qualifications = new List<Qualification>();

            foreach (var providerData in _providersData)
            {
                if (providerData.Key == "qualifications")
                {
                    var qualificationsDictionary = _jsonConvertor.DeserializeObject<IDictionary<int, string>>(providerData.Value.ToString());

                    foreach (var qualification in qualificationsDictionary)
                    {
                        qualifications.Add(new Qualification { Id = qualification.Key, Name = qualification.Value });
                    }
                }
            }

            return qualifications.AsQueryable();
        }

        private IQueryable<Provider> GetAllProviders()
        {
            var providers = new List<Provider>();

            foreach (var providerData in _providersData)
            {
                if (providerData.Key == "providers")
                {
                    providers = _jsonConvertor.DeserializeObject<List<Provider>>(providerData.Value.ToString());
                }
            }

            return providers.AsQueryable();
        }

    }
}

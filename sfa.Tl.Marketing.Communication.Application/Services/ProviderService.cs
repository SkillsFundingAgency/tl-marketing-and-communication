using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models.Configuration;
using sfa.Tl.Marketing.Communication.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sfa.Tl.Marketing.Communication.Application.Services
{
    public class ProviderService : IProviderService
    {
        private readonly IFileReader _fileReader;
        private readonly IJsonConvertor _jsonConvertor;
        private readonly ConfigurationOptions _configurationOptions;
        private readonly IQueryable<Provider> _providers;

        public ProviderService(IFileReader fileReader, IJsonConvertor jsonConvertor, ConfigurationOptions configurationOptions)
        {
            _fileReader = fileReader;
            _jsonConvertor = jsonConvertor;
            _configurationOptions = configurationOptions;
            _providers = GetAllProviders();
        }

        public IQueryable<Provider> GetProviders()
        {
            return _providers;
        }

        private IQueryable<Provider> GetAllProviders()
        {
            var json = _fileReader.ReadAllText(_configurationOptions.DataFilePath);
            var providers = _jsonConvertor.DeserializeObject<IEnumerable<Provider>>(json);
            return providers.AsQueryable();
        }
    }
}

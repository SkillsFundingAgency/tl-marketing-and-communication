using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models.Configuration;
using sfa.Tl.Marketing.Communication.Models.Dto;
using System.Collections.Generic;
using System.Linq;

namespace sfa.Tl.Marketing.Communication.Application.Services
{
    public class QualificationService : IQualificationService
    {
        private readonly IFileReader _fileReader;
        private readonly IJsonConvertor _jsonConvertor;
        private readonly ConfigurationOptions _configurationOptions;
        private readonly IQueryable<Qualification> _qualifications;

        public QualificationService(IFileReader fileReader, IJsonConvertor jsonConvertor, ConfigurationOptions configurationOptions)
        {
            _fileReader = fileReader;
            _jsonConvertor = jsonConvertor;
            _configurationOptions = configurationOptions;
            _qualifications = GetAllQualifications();
        }

        public IEnumerable<Qualification> GetQualifications(int[] qualificationIds)
        {
            return _qualifications.Where(q => qualificationIds.Contains(q.Id));
        }

        public IEnumerable<Qualification> GetQualifications()
        {
            return _qualifications;
        }

        private IQueryable<Qualification> GetAllQualifications()
        {
            var json = _fileReader.ReadAllText(_configurationOptions.DataFilePath);
            var qualifications = _jsonConvertor.DeserializeObject<IEnumerable<Qualification>>(json);
            return qualifications.AsQueryable();
        }
    }
}

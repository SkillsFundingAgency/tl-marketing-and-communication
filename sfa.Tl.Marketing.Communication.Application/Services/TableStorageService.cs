using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models.Dto;
using sfa.Tl.Marketing.Communication.Models.Entities;

namespace sfa.Tl.Marketing.Communication.Application.Services
{
    public class TableStorageService : ITableStorageService
    {
        private readonly ICloudTableRepository<ProviderEntity> _providerRepository;
        private readonly ICloudTableRepository<QualificationEntity> _qualificationRepository;
        private readonly ILogger<TableStorageService> _logger;

        public TableStorageService(
            ICloudTableRepository<ProviderEntity> providerRepository,
            ICloudTableRepository<QualificationEntity> qualificationRepository,
            ILogger<TableStorageService> logger)
        {
            _providerRepository = providerRepository ?? throw new ArgumentNullException(nameof(providerRepository));
            _qualificationRepository = qualificationRepository ?? throw new ArgumentNullException(nameof(qualificationRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> ClearProviders()
        {
            return await _providerRepository.DeleteAll();
        }

        public async Task<int> SaveProviders(IList<Provider> providers)
        {
            if (providers == null || !providers.Any())
            {
                return 0;
            }

            var providerEntities = providers
                .Select(provider =>
                    new ProviderEntity
                    {
                        Id = provider.Id,
                        Name = provider.Name,
                        Locations = provider.Locations.Select(
                            location => 
                                new LocationEntity
                                {
                                    Name = location.Name,
                                    Postcode = location.Postcode,
                                    Latitude = location.Latitude,
                                    Longitude = location.Longitude,
                                    Town = location.Town,
                                    Website = location.Website,
                                    DeliveryYears = location.DeliveryYears.Select(
                                        deliveryYear => 
                                            new DeliveryYearEntity
                                            {
                                                Year = deliveryYear.Year,
                                                Qualifications = deliveryYear.Qualifications.ToList()
                                            }
                                        ).ToList(),
                                }).ToList()
                    }).ToList();

            var saved = await _providerRepository.Save(providerEntities);

            _logger.LogInformation($"SaveProviders saved {saved} records.");
            return saved;
        }

        public async Task<IList<Provider>> RetrieveProviders()
        {
            var providerEntities = await _providerRepository.GetAll();

            var providers = providerEntities
                .Select(provider =>
                    new Provider
                    {
                        Id = provider.Id,
                        Name = provider.Name,
                        Locations = provider.Locations.Select(
                            location =>
                                new Location
                                {
                                    Name = location.Name,
                                    Postcode = location.Postcode,
                                    Latitude = location.Latitude,
                                    Longitude = location.Longitude,
                                    Town = location.Town,
                                    Website = location.Website,
                                    DeliveryYears = location.DeliveryYears.Select(
                                        deliveryYear =>
                                            new DeliveryYearDto
                                            {
                                                Year = deliveryYear.Year,
                                                Qualifications = deliveryYear.Qualifications.ToList()
                                            }).ToList()
                                }).ToList()
                    }).ToList();

            _logger.LogInformation($"RetrieveProviders found {providers.Count()} records.");
            return providers;
        }

        public async  Task<int> ClearQualifications()
        {
            return await _qualificationRepository.DeleteAll();
        }

        public async Task<int> SaveQualifications(IList<Qualification> qualifications)
        {
            if (qualifications == null || !qualifications.Any())
            {
                return 0;
            }

            var qualificationEntities = qualifications
                .Select(qualification =>
                    new QualificationEntity
                    {
                        Id = qualification.Id,
                        Name = qualification.Name
                    }).ToList();

            var saved = await _qualificationRepository.Save(qualificationEntities);

            _logger.LogInformation($"SaveQualifications saved {saved} records.");
            return saved;
        }

        public async Task<IList<Qualification>> RetrieveQualifications()
        {
            var qualificationEntities = await _qualificationRepository.GetAll();

            var qualifications = qualificationEntities
                .Select(q =>
                    new Qualification
                    {
                        Id = q.Id,
                        Name = q.Name
                    }).ToList();

            _logger.LogInformation($"RetrieveQualifications found {qualifications.Count} records.");

            return qualifications;
        }
    }
}

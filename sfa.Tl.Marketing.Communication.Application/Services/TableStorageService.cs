using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models.Dto;
using sfa.Tl.Marketing.Communication.Models.Entities;
using sfa.Tl.Marketing.Communication.Models.Extensions;

namespace sfa.Tl.Marketing.Communication.Application.Services
{
    public class TableStorageService : ITableStorageService
    {
        private readonly ICloudTableRepository<LocationEntity> _locationRepository;
        private readonly ICloudTableRepository<ProviderEntity> _providerRepository;
        private readonly ICloudTableRepository<QualificationEntity> _qualificationRepository;
        private readonly ILogger<TableStorageService> _logger;

        public TableStorageService(
            ICloudTableRepository<LocationEntity> locationRepository,
            ICloudTableRepository<ProviderEntity> providerRepository,
            ICloudTableRepository<QualificationEntity> qualificationRepository,
            ILogger<TableStorageService> logger)
        {
            _locationRepository = locationRepository ?? throw new ArgumentNullException(nameof(locationRepository));
            _providerRepository = providerRepository ?? throw new ArgumentNullException(nameof(providerRepository));
            _qualificationRepository = qualificationRepository ?? throw new ArgumentNullException(nameof(qualificationRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> ClearProviders()
        {
            var deletedProviders = await _providerRepository.DeleteAll();
            await _locationRepository.DeleteTable();
            return deletedProviders;
        }

        public async Task<int> RemoveProviders(IList<Provider> providers)
        {
            if (providers == null || !providers.Any())
            {
                return 0;
            }

            foreach (var provider in providers)
            {
                await _locationRepository.DeleteByPartitionKey(provider.UkPrn.ToString());
            }

            var providerEntities = providers.ToProviderEntityList();

            return await _providerRepository.Delete(providerEntities);
        }

        public async Task<int> SaveProviders(IList<Provider> providers)
        {
            if (providers == null || !providers.Any())
            {
                return 0;
            }

            var providerEntities = providers.ToProviderEntityList();

            var saved = await _providerRepository.Save(providerEntities);

            var savedLocations = 0;
            var deletedLocations = 0;
            foreach (var provider in providers)
            {
                deletedLocations += await _locationRepository.DeleteByPartitionKey(provider.UkPrn.ToString());

                savedLocations += await _locationRepository
                    .Save(provider.Locations.ToLocationEntityList(provider.UkPrn.ToString()));
            }

            _logger.LogInformation("SaveProviders saved " +
                                   $"{saved} providers, " +
                                   $"and saved {savedLocations} " +
                                   $"and deleted {deletedLocations} locations.");
            return saved;
        }

        public async Task<IList<Provider>> GetAllProviders()
        {
            var providerEntities = await _providerRepository.GetAll();

            var providers = providerEntities.ToProviderList();

            var locationCount = 0;
            foreach (var provider in providers)
            {
                var locationEntities = await _locationRepository.GetByPartitionKey(provider.UkPrn.ToString());
                provider.Locations = locationEntities.ToLocationList();
                locationCount += locationEntities.Count;
            }

            _logger.LogInformation($"TableStorageService::RetrieveProviders found {providers.Count} providers with {locationCount} locations.");
            return providers;
        }

        public async Task<int> ClearQualifications()
        {
            return await _qualificationRepository.DeleteAll();
        }

        public async Task<int> RemoveQualifications(IList<Qualification> qualifications)
        {
            if (qualifications == null || !qualifications.Any())
            {
                return 0;
            }

            var qualificationEntities = qualifications.ToQualificationEntityList();

            return await _qualificationRepository.Delete(qualificationEntities);
        }

        public async Task<int> SaveQualifications(IList<Qualification> qualifications)
        {
            if (qualifications == null || !qualifications.Any())
            {
                return 0;
            }

            var qualificationEntities = qualifications.ToQualificationEntityList();

            var saved = await _qualificationRepository.Save(qualificationEntities);

            _logger.LogInformation($"SaveQualifications saved {saved} records.");
            return saved;
        }

        public async Task<IList<Qualification>> GetAllQualifications()
        {
            var qualifications =
                (await _qualificationRepository.GetAll())
                .ToQualificationList();

            _logger.LogInformation($"RetrieveQualifications found {qualifications.Count} records.");

            return qualifications;
        }
    }
}

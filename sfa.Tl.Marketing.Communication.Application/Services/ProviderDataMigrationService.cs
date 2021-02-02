using System;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace sfa.Tl.Marketing.Communication.Application.Services
{
    public class ProviderDataMigrationService : IProviderDataMigrationService
    {
        private readonly IFileReader _fileReader;
        private readonly ITableStorageService _tableStorageService;
        private readonly ILogger<ProviderDataMigrationService> _logger;

        public ProviderDataMigrationService(
            IFileReader fileReader,
            ITableStorageService tableStorageService,
            ILogger<ProviderDataMigrationService> logger)
        {
            _fileReader = fileReader ?? throw new ArgumentNullException(nameof(fileReader));
            _tableStorageService = tableStorageService ?? throw new ArgumentNullException(nameof(tableStorageService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> WriteQualifications(string qualificationsFilePath)
        {
            if (string.IsNullOrEmpty(qualificationsFilePath))
                return 0;

            var removedQualifications = await _tableStorageService.ClearQualifications();
            _logger.LogInformation($"Removed {removedQualifications} qualifications from table storage");

            var savedQualifications = await _tableStorageService
                .SaveQualifications(await GetQualificationsData(_fileReader, qualificationsFilePath));
            _logger.LogInformation($"Saved {savedQualifications} qualifications to table storage");

            return savedQualifications;
        }

        public async Task<int> WriteProviders(string providersFilePath)
        {
            if (string.IsNullOrEmpty(providersFilePath))
                return 0;

            var removedProviders = await _tableStorageService.ClearProviders();
            _logger.LogInformation($"Removed {removedProviders} providers from table storage");

            var savedProviders = await _tableStorageService
                .SaveProviders(await GetProvidersData(_fileReader, providersFilePath));
            _logger.LogInformation($"Saved {savedProviders} providers to table storage");

            return savedProviders;
        }

        private static async Task<IList<Provider>> GetProvidersData(IFileReader fileReader, string providersDataFilePath)
        {
            var json = await fileReader.ReadAllTextAsync(providersDataFilePath);
            return JsonDocument.Parse(json)
                .RootElement
                .GetProperty("providers")
                .EnumerateArray()
                .Select(p =>
                    new Provider
                    {
                        Id = p.GetProperty("id").GetInt32(),
                        UkPrn = p.GetProperty("ukPrn").GetInt64(),
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
                                                new DeliveryYearDto
                                                {
                                                    Year = d.GetProperty("year").GetInt16(),
                                                    Qualifications = d.GetProperty("qualifications")
                                                        .EnumerateArray()
                                                        .Select(q => q.GetInt32())
                                                        .ToList()
                                                })
                                            .ToList()
                                        : new List<DeliveryYearDto>()
                                }).ToList()
                    })
                .ToList();
        }

        private static async Task<IList<Qualification>> GetQualificationsData(IFileReader fileReader, string inputFilePath)
        {
            var json = await fileReader.ReadAllTextAsync(inputFilePath);
            return JsonDocument.Parse(json)
                .RootElement
                .GetProperty("qualifications")
                .EnumerateObject()
                .Select(q =>
                    new Qualification
                    {
                        Id = int.Parse(q.Name),
                        Name = q.Value.GetString()
                    })
                .OrderBy(q => q.Id)
                .ToList();
        }
    }
}

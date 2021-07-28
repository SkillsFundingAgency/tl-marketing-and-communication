using System.IO;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Application.Repositories;
using sfa.Tl.Marketing.Communication.Application.Services;
using sfa.Tl.Marketing.Communication.Models.Entities;

namespace sfa.Tl.Marketing.Communication.Benchmarks
{
    public static class Helpers
    {
        public static IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true)
                .AddJsonFile("appsettings.Development.json", true);

            var configuration = builder.Build();

            return configuration;
        }

        public static ITableStorageService CreateTableStorageService(
            string tableStorageConnectionString,
            ILoggerFactory loggerFactory)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(tableStorageConnectionString);

            var cloudTableClient = cloudStorageAccount.CreateCloudTableClient();

            var locationRepository = new GenericCloudTableRepository<LocationEntity>(
                cloudTableClient,
                loggerFactory.CreateLogger<GenericCloudTableRepository<LocationEntity>>());

            var providerRepository = new GenericCloudTableRepository<ProviderEntity>(
                cloudTableClient,
                loggerFactory.CreateLogger<GenericCloudTableRepository<ProviderEntity>>());

            var qualificationRepository = new GenericCloudTableRepository<QualificationEntity>(
                cloudTableClient,
                loggerFactory.CreateLogger<GenericCloudTableRepository<QualificationEntity>>());

            return new TableStorageService(
                locationRepository,
                providerRepository,
                qualificationRepository,
                loggerFactory.CreateLogger<TableStorageService>());
        }
    }
}

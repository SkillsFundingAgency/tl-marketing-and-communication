using System;
using System.IO;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Application.Repositories;
using sfa.Tl.Marketing.Communication.Application.Services;
using sfa.Tl.Marketing.Communication.DataLoad.Services;
using sfa.Tl.Marketing.Communication.Models.Entities;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", true)
    .AddJsonFile("appsettings.development.json", true);

var configuration = builder.Build();

var tableStorageConnectionString = configuration.GetValue<string>("TableStorageConnectionString");
if (!string.IsNullOrEmpty(tableStorageConnectionString))
{
    var loggerFactory = new LoggerFactory();
    var providerDataMigrationService = new ProviderDataMigrationService(
        new FileReader(),
        CreateTableStorageService(tableStorageConnectionString, loggerFactory),
        loggerFactory.CreateLogger<ProviderDataMigrationService>());

    var qualificationsSaved = await providerDataMigrationService
        .WriteQualifications(configuration.GetValue<string>("QualificationJsonInputFilePath"));
    Console.WriteLine("");
    Console.WriteLine($"Copied {qualificationsSaved} qualifications to table storage.");

    var providersSaved = await providerDataMigrationService
        .WriteProviders(configuration.GetValue<string>("ProviderJsonInputFilePath"));
    Console.WriteLine($"Copied {providersSaved} providers to table storage.");
}

static ITableStorageService CreateTableStorageService(
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
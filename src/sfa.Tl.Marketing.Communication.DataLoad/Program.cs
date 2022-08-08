﻿using System;
using System.IO;
using Azure.Data.Tables;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Application.Repositories;
using sfa.Tl.Marketing.Communication.Application.Services;
using sfa.Tl.Marketing.Communication.DataLoad.Services;
using sfa.Tl.Marketing.Communication.Models.Configuration;
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
        .WriteQualificationsToTableStorage(configuration.GetValue<string>("QualificationJsonInputFilePath"));
    Console.WriteLine("");
    Console.WriteLine($"Copied {qualificationsSaved} qualifications to table storage.");

    var providersSaved = await providerDataMigrationService
        .WriteProvidersToTableStorage(configuration.GetValue<string>("ProviderJsonInputFilePath"));
    Console.WriteLine($"Copied {providersSaved} providers to table storage.");
}

static ITableStorageService CreateTableStorageService(
    string tableStorageConnectionString,
    ILoggerFactory loggerFactory)
{
    var tableServiceClient = new TableServiceClient(tableStorageConnectionString);

    var siteConfiguration = new ConfigurationOptions
    {
        Environment = "Data Load"
    };

    var locationRepository = new GenericCloudTableRepository<LocationEntity>(
        tableServiceClient,
        siteConfiguration,
        loggerFactory.CreateLogger<GenericCloudTableRepository<LocationEntity>>());

    var providerRepository = new GenericCloudTableRepository<ProviderEntity>(
        tableServiceClient,
        siteConfiguration,
        loggerFactory.CreateLogger<GenericCloudTableRepository<ProviderEntity>>());

    var qualificationRepository = new GenericCloudTableRepository<QualificationEntity>(
        tableServiceClient,
        siteConfiguration,
        loggerFactory.CreateLogger<GenericCloudTableRepository<QualificationEntity>>());

    return new TableStorageService(
        locationRepository,
        providerRepository,
        qualificationRepository,
        loggerFactory.CreateLogger<TableStorageService>());
}
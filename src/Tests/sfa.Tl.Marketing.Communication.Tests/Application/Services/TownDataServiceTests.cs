﻿using sfa.Tl.Marketing.Communication.Application.Services;
using sfa.Tl.Marketing.Communication.Tests.Common.Extensions;
using sfa.Tl.Marketing.Communication.Models.Dto;
using sfa.Tl.Marketing.Communication.UnitTests.Builders;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using sfa.Tl.Marketing.Communication.Application.Caching;
using sfa.Tl.Marketing.Communication.Models.Configuration;

namespace sfa.Tl.Marketing.Communication.UnitTests.Application.Services;

public class TownDataServiceTests
{
    private const string FirstPageUriString = $"{TownDataService.OfficeForNationalStatisticsLocationUrl}&resultRecordCount=2000&resultOffSet=0";
    private const string ThirdPageUriString = $"{TownDataService.OfficeForNationalStatisticsLocationUrl}&resultRecordCount=999&resultOffSet=2";

    [Fact]
    public void Constructor_Guards_Against_Null_Parameters()
    {
        typeof(TownDataService)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void Constructor_Guards_Against_Bad_Parameters()
    {
        typeof(TownDataService)
            .ShouldNotAcceptNullOrBadConstructorArguments();
    }

    [Fact]
    public void GetUri_Returns_Expected_Value()
    {
        var uri = TownDataService.GetUri(2, 999);

        uri.Should().NotBeNull();
        uri.AbsoluteUri.Should().Be(ThirdPageUriString);
    }

    [Fact]
    public async Task ImportTowns_Creates_Expected_Number_Of_Towns()
    {
        var responses = new Dictionary<string, string>
        {
            { FirstPageUriString, new NationalStatisticsJsonBuilder().BuildNationalStatisticsLocationsResponse() }
        };

        IList<Town> receivedTowns = null;

        var tableStorageService = Substitute.For<ITableStorageService>();
        await tableStorageService
            .SaveTowns(Arg.Do<IList<Town>>(
                x => receivedTowns = x?.ToList()));

        var service = new TownDataServiceBuilder()
            .Build(responses,
                tableStorageService);

        await service.ImportTowns();

        receivedTowns.Should().NotBeNull();
        receivedTowns.Count.Should().Be(5);
    }

    [Fact]
    public async Task ImportTowns_Creates_Expected_Towns()
    {
        var responses = new Dictionary<string, string>
        {
            { FirstPageUriString, new NationalStatisticsJsonBuilder().BuildNationalStatisticsLocationsResponse() }
        };

        IList<Town> receivedTowns = null;

        var tableStorageService = Substitute.For<ITableStorageService>();
        await tableStorageService
            .SaveTowns(Arg.Do<IList<Town>>(
                x => receivedTowns = x?.ToList()));

        var service = new TownDataServiceBuilder()
            .Build(responses,
                tableStorageService);

        await service.ImportTowns();

        receivedTowns.Should().NotBeNull();
        receivedTowns.Count.Should().Be(5);

        // ReSharper disable StringLiteralTypo
        ValidateTown(receivedTowns
                .SingleOrDefault(t =>
                    t.Id == 302),
            302,
            "Abingdon",
            "Oxfordshire",
            "Oxfordshire",
            51.674302,
            -1.282302,
            "abingdonoxfordshire");

        ValidateTown(receivedTowns
                .SingleOrDefault(t =>
                    t.Id == 304),
            304,
            "Abingdon",
            "Inner London",
            "Greater London",
            51.497681,
            -0.192782,
            "abingdoninnerlondon");

        ValidateTown(receivedTowns
                .SingleOrDefault(t =>
                    t.Id == 72832),
            72832,
            "West Bromwich",
            "West Midlands",
            "West Midlands",
            52.530629,
            -2.005941,
            "westbromwichwestmidlands");

        ValidateTown(receivedTowns
                .SingleOrDefault(t =>
                    t.Id == 72834),
            72834,
            "West Bromwich (East)",
            "West Midlands",
            "West Midlands",
            52.540693,
            -1.942085,
            "westbromwicheastwestmidlands");

        ValidateTown(receivedTowns
                .SingleOrDefault(t =>
                    t.Id == 72835),
            72835,
            "West Bromwich Central",
            "West Midlands",
            "West Midlands",
            52.520416,
            -1.984158,
            "westbromwichcentralwestmidlands");
        // ReSharper restore StringLiteralTypo
    }

    [Fact]
    public async Task ImportTowns_Filters_Out_Civil_Parishes()
    {
        var responses = new Dictionary<string, string>
        {
            { FirstPageUriString, new NationalStatisticsJsonBuilder().BuildNationalStatisticsLocationsResponse() }
        };

        IList<Town> receivedTowns = null;

        var tableStorageService = Substitute.For<ITableStorageService>();
        await tableStorageService
            .SaveTowns(Arg.Do<IList<Town>>(
                x => receivedTowns = x?.ToList()));

        var service = new TownDataServiceBuilder()
            .Build(responses,
                tableStorageService);

        await service.ImportTowns();

        // ReSharper disable StringLiteralTypo
        receivedTowns.Should().NotContain(t =>
                t.Name == "Abbas and Templecombe" ||
                t.Name == "Abberley");
        // ReSharper restore StringLiteralTypo
    }

    [Fact]
    public async Task ImportTowns_From_Csv_Stream_Creates_Expected_Number_Of_Towns()
    {
        IList<Town> receivedTowns = null;

        var tableStorageService = Substitute.For<ITableStorageService>();
        await tableStorageService
            .SaveTowns(Arg.Do<IList<Town>>(
                x => receivedTowns = x?.ToList()));

        var stream = new IndexOfPlaceNamesCsvBuilder().BuildIndexOfPlaceNamesCsvAsStream();

        var service = new TownDataServiceBuilder()
            .Build(tableStorageService: tableStorageService);

        await service.ImportTowns(stream);

        receivedTowns.Should().NotBeNull();
        receivedTowns.Count.Should().Be(6);
    }

    [Fact]
    public async Task ImportTowns_From_Csv_Stream_Creates_Expected_Towns()
    {
        IList<Town> receivedTowns = null;

        var tableStorageService = Substitute.For<ITableStorageService>();
        await tableStorageService
            .SaveTowns(Arg.Do<IList<Town>>(
                x => receivedTowns = x?.ToList()));

        var stream = new IndexOfPlaceNamesCsvBuilder().BuildIndexOfPlaceNamesCsvAsStream();

        var service = new TownDataServiceBuilder()
            .Build(tableStorageService: tableStorageService);
        
        await service.ImportTowns(stream);

        receivedTowns.Should().NotBeNull();
        receivedTowns.Count.Should().Be(6);

        // ReSharper disable StringLiteralTypo
        ValidateTown(receivedTowns
                .SingleOrDefault(t =>
                    t.Id == 302),
            302,
            "Abingdon",
            "Oxfordshire",
            "Oxfordshire",
            51.674302,
            -1.282302,
            "abingdonoxfordshire");

        ValidateTown(receivedTowns
                .SingleOrDefault(t =>
                    t.Id == 304),
            304,
            "Abingdon",
            "Inner London",
            "Greater London",
            51.497681,
            -0.192782,
            "abingdoninnerlondon");

        ValidateTown(receivedTowns
                .SingleOrDefault(t =>
                    t.Id == 10214),
            10214,
            "Bude",
            null,
            "Cornwall",
            50.825207,
            -4.538982,
            "budecornwall");

        ValidateTown(receivedTowns
                .SingleOrDefault(t =>
                    t.Id == 72832),
            72832,
            "West Bromwich",
            "West Midlands",
            "West Midlands",
            52.530629,
            -2.005941,
            "westbromwichwestmidlands");

        ValidateTown(receivedTowns
                .SingleOrDefault(t =>
                    t.Id == 72834),
            72834,
            "West Bromwich (East)",
            "West Midlands",
            "West Midlands",
            52.540693,
            -1.942085,
            "westbromwicheastwestmidlands");

        ValidateTown(receivedTowns
                .SingleOrDefault(t =>
                    t.Id == 72835),
            72835,
            "West Bromwich Central",
            "West Midlands",
            "West Midlands",
            52.520416,
            -1.984158,
            "westbromwichcentralwestmidlands");
        // ReSharper restore StringLiteralTypo
    }

    [Fact]
    public async Task ImportTowns_From_Csv_Stream_Creates_Expected_Town_With_Null_County()
    {
        IList<Town> receivedTowns = null;

        var tableStorageService = Substitute.For<ITableStorageService>();
        await tableStorageService
            .SaveTowns(Arg.Do<IList<Town>>(
                x => receivedTowns = x?.ToList()));

        var stream = new IndexOfPlaceNamesCsvBuilder().BuildIndexOfPlaceNamesCsvAsStream();

        var service = new TownDataServiceBuilder()
            .Build(tableStorageService: tableStorageService);

        await service.ImportTowns(stream);

        receivedTowns.Should().NotBeNullOrEmpty();

        // ReSharper disable StringLiteralTypo
        ValidateTown(receivedTowns
                .SingleOrDefault(t =>
                    t.Id == 10214),
            10214,
            "Bude",
            null,
            "Cornwall",
            50.825207,
            -4.538982,
            "budecornwall");
        // ReSharper restore StringLiteralTypo
    }

    [Fact]
    public async Task ImportTowns_From_Csv_Stream_From_Csv_Stream_Filters_Out_Civil_Parishes()
    {
        IList<Town> receivedTowns = null;

        var tableStorageService = Substitute.For<ITableStorageService>();
        await tableStorageService
            .SaveTowns(Arg.Do<IList<Town>>(
                x => receivedTowns = x?.ToList()));

        var stream = new IndexOfPlaceNamesCsvBuilder().BuildIndexOfPlaceNamesCsvAsStream();

        var service = new TownDataServiceBuilder()
            .Build(tableStorageService: tableStorageService);

        await service.ImportTowns(stream);

        // ReSharper disable StringLiteralTypo
        receivedTowns.Should().NotContain(t =>
                t.Name == "Abbas and Templecombe" ||
                t.Name == "Abberley");
        // ReSharper restore StringLiteralTypo
    }

    [Fact]
    public async Task ImportTowns_From_Csv_Stream_From_Csv_Stream_Filters_Out_Non_English_Towns()
    {
        IList<Town> receivedTowns = null;

        var tableStorageService = Substitute.For<ITableStorageService>();
        await tableStorageService
            .SaveTowns(Arg.Do<IList<Town>>(
                x => receivedTowns = x?.ToList()));

        var stream = new IndexOfPlaceNamesCsvBuilder().BuildIndexOfPlaceNamesCsvAsStream();

        var service = new TownDataServiceBuilder()
            .Build(tableStorageService: tableStorageService);

        await service.ImportTowns(stream);

        // ReSharper disable once StringLiteralTypo
        receivedTowns.Should().NotContain(t => t.Name == "Aberdovey");
    }

    [Fact]
    public async Task ImportTowns_From_Csv_Stream_Deduplicates_Abingdon_Correctly()
    {
        IList<Town> receivedTowns = null;

        var tableStorageService = Substitute.For<ITableStorageService>();
        await tableStorageService
            .SaveTowns(Arg.Do<IList<Town>>(
                x => receivedTowns = x?.ToList()));

        var stream = new IndexOfPlaceNamesCsvBuilder().BuildIndexOfPlaceNamesCsvAsStream();

        var service = new TownDataServiceBuilder()
            .Build(tableStorageService: tableStorageService);

        await service.ImportTowns(stream);

        receivedTowns.Should().NotBeNull();

        var abingdonInstances
            = receivedTowns.Where(t => t.Name == "Abingdon");
        var abingdonInOxfordshire = receivedTowns
            .Where(t => t.Name == "Abingdon" && t.County == "Oxfordshire")
            .ToList();

        abingdonInstances.Count().Should().Be(2);
        abingdonInOxfordshire.Count.Should().Be(1);
        
        // ReSharper disable StringLiteralTypo
        ValidateTown(abingdonInOxfordshire.Single(),
            302,
            "Abingdon",
            "Oxfordshire",
            "Oxfordshire",
            51.674302,
            -1.282302,
            "abingdonoxfordshire");
        // ReSharper restore StringLiteralTypo
    }

    [Fact]
    public async Task ImportTowns_From_Csv_Stream_Deduplicates_WestBromwich_Correctly()
    {
        IList<Town> receivedTowns = null;

        var tableStorageService = Substitute.For<ITableStorageService>();
        await tableStorageService
            .SaveTowns(Arg.Do<IList<Town>>(
                x => receivedTowns = x?.ToList()));

        var stream = new IndexOfPlaceNamesCsvBuilder().BuildIndexOfPlaceNamesCsvAsStream();

        var service = new TownDataServiceBuilder()
            .Build(tableStorageService: tableStorageService);

        await service.ImportTowns(stream);

        receivedTowns.Should().NotBeNull();

        var westBromwich = receivedTowns
            .Where(t => t.Name == "West Bromwich")
            .ToList();

        westBromwich.Count.Should().Be(1);
        
        // ReSharper disable StringLiteralTypo
        ValidateTown(westBromwich.Single(),
            72832,
            "West Bromwich",
            "West Midlands",
            "West Midlands",
            52.530629,
            -2.005941,
            "westbromwichwestmidlands");
        // ReSharper restore StringLiteralTypo
    }

    [Fact]
    public async Task ImportTowns_Deduplicates_Abingdon_Correctly()
    {
        var responses = new Dictionary<string, string>
        {
            { FirstPageUriString, new NationalStatisticsJsonBuilder().BuildNationalStatisticsLocationsResponse() }
        };

        IList<Town> receivedTowns = null;

        var tableStorageService = Substitute.For<ITableStorageService>();
        await tableStorageService
            .SaveTowns(Arg.Do<IList<Town>>(
                x => receivedTowns = x?.ToList()));

        var service = new TownDataServiceBuilder()
            .Build(responses,
                tableStorageService);

        await service.ImportTowns();

        receivedTowns.Should().NotBeNull();

        var abingdonInstances
            = receivedTowns.Where(t => t.Name == "Abingdon");
        var abingdonInOxfordshire = receivedTowns
            .Where(t => t.Name == "Abingdon" && t.County == "Oxfordshire")
            .ToList();

        abingdonInstances.Count().Should().Be(2);
        abingdonInOxfordshire.Count.Should().Be(1);

        ValidateTown(abingdonInOxfordshire.Single(),
            302,
            "Abingdon",
            "Oxfordshire",
            "Oxfordshire",
            //"Vale of White Horse",
            //"NMD",
            51.674302,
            -1.282302,
            // ReSharper disable once StringLiteralTypo
            "abingdonoxfordshire");
    }

    [Fact]
    public async Task ImportTowns_Deduplicates_WestBromwich_Correctly()
    {
        var responses = new Dictionary<string, string>
        {
            { FirstPageUriString, new NationalStatisticsJsonBuilder().BuildNationalStatisticsLocationsResponse() }
        };

        IList<Town> receivedTowns = null;

        var tableStorageService = Substitute.For<ITableStorageService>();
        await tableStorageService
            .SaveTowns(Arg.Do<IList<Town>>(
                x => receivedTowns = x?.ToList()));

        var service = new TownDataServiceBuilder()
            .Build(responses,
                tableStorageService);

        await service.ImportTowns();

        receivedTowns.Should().NotBeNull();

        var westBromwich = receivedTowns
            .Where(t => t.Name == "West Bromwich")
            .ToList();

        westBromwich.Count.Should().Be(1);

        ValidateTown(westBromwich.Single(),
            72832,
            "West Bromwich",
            "West Midlands",
            "West Midlands",
            52.530629,
            -2.005941,
            // ReSharper disable once StringLiteralTypo
            "westbromwichwestmidlands");
    }

    [Fact]
    public async Task IsSearchTermValid_Returns_True_For_Matching_Town()
    {
        const string searchTerm = "Test";
        var partitionKey = searchTerm[..3].ToUpper();

        var towns = new TownListBuilder()
            .Add(10)
            .Build()
            .ToList();

        var tableStorageService = Substitute.For<ITableStorageService>();
        tableStorageService
            .GetTownsByPartitionKey(partitionKey)
            .Returns(towns);

        var service = new TownDataServiceBuilder()
            .Build(tableStorageService: tableStorageService);

        var result = await service
            .IsSearchTermValid(searchTerm);

        result.IsValid.Should().BeTrue();

        await tableStorageService
            .Received(1)
            .GetTownsByPartitionKey(partitionKey);
    }

    [Fact]
    public async Task IsSearchTermValid_Returns_False_For_Non_Matching_Town()
    {
        const string searchTerm = "Unknown";
        var partitionKey = searchTerm[..3].ToUpper();

        var towns = new TownListBuilder()
            .Add(10)
            .Build()
            .ToList();

        var tableStorageService = Substitute.For<ITableStorageService>();
        tableStorageService
            .GetTownsByPartitionKey(partitionKey)
            .Returns(towns);

        var service = new TownDataServiceBuilder()
            .Build(tableStorageService: tableStorageService);

        var result = await service
            .IsSearchTermValid(searchTerm);

        result.IsValid.Should().BeFalse();

        await tableStorageService
            .Received(1)
            .GetTownsByPartitionKey(partitionKey);
    }

    [Fact]
    public async Task Search_Calls_Repository_And_Returns_Results()
    {
        const string searchTerm = "Test";
        var partitionKey = searchTerm[..3].ToUpper();
        const int maxResults = 10;

        var towns = new TownListBuilder()
            .Add(10)
            .Build()
            .ToList();

        var tableStorageService = Substitute.For<ITableStorageService>();
        tableStorageService
            .GetTownsByPartitionKey(partitionKey)
            .Returns(towns);

        var service = new TownDataServiceBuilder()
            .Build(tableStorageService: tableStorageService);

        var result = await service
            .Search(searchTerm, maxResults);

        result.Should().BeEquivalentTo(towns);

        await tableStorageService
            .Received(1)
            .GetTownsByPartitionKey(partitionKey);
    }

    [Fact]
    public async Task Search_Returns_No_More_Than_Repository_With_Max_Results()
    {
        const string searchTerm = "Test";
        var partitionKey = searchTerm[..3].ToUpper();
        const int maxResults = 3;

        var towns = new TownListBuilder()
            .Add(10)
            .Build()
            .ToList();

        var tableStorageService = Substitute.For<ITableStorageService>();
        tableStorageService
            .GetTownsByPartitionKey(partitionKey)
            .Returns(towns);

        var service = new TownDataServiceBuilder()
            .Build(tableStorageService: tableStorageService);

        var result = await service
            .Search(searchTerm, maxResults);

        result.Count().Should().Be(maxResults);

        await tableStorageService
            .Received(1)
            .GetTownsByPartitionKey(partitionKey);
    }

    [Fact]
    public async Task Search_Does_Not_Call_Repository__And_Returns_Cache_Results_When_Partition_Is_Cached()
    {
        const string searchTerm = "Test";
        var partitionKey = searchTerm[..3].ToUpper();
        const int maxResults = 20;

        var towns = new TownListBuilder()
            .Add(10)
            .Build()
            .ToList();

        var configuration = new ConfigurationOptions
        {
            CacheExpiryInSeconds = 30
        };

        var cache = Substitute.For<IMemoryCache>();
        var cacheKey = CacheKeys.TownPartitionKey(partitionKey);

        cache.TryGetValue(cacheKey, out Arg.Any<IList<Town>>())
            .Returns(x =>
            {
                x[1] = towns;
                return true;
            });

        var tableStorageService = Substitute.For<ITableStorageService>();

        var service = new TownDataServiceBuilder()
            .Build(tableStorageService: tableStorageService,
                cache: cache,
                configuration: configuration);

        var result = await service
            .Search(searchTerm, maxResults);

        result.Should().BeEquivalentTo(towns);

        await tableStorageService
            .DidNotReceive()
            .GetTownsByPartitionKey(Arg.Any<string>());
    }

    [Fact]
    public async Task Search_Does_Not_Call_Repository_For_Postcode()
    {
        const string searchTerm = "CV1 2WT";
        const int maxResults = 20;

        var tableStorageService = Substitute.For<ITableStorageService>();

        var service = new TownDataServiceBuilder()
            .Build(tableStorageService: tableStorageService);

        var result = await service
            .Search(searchTerm, maxResults);

        result.Should().BeEmpty();

        await tableStorageService
            .DidNotReceive()
            .GetTownsByPartitionKey(Arg.Any<string>());
    }

    private static void ValidateTown(Town town,
        int id,
        string name,
        string county,
        string localAuthority,
        double latitude,
        double longitude,
        string searchString)
    {
        town.Should().NotBeNull();
        town.Id.Should().Be(id);
        town.Name.Should().Be(name);
        town.County.Should().Be(county);
        town.LocalAuthority.Should().Be(localAuthority);
        town.Latitude.Should().Be(latitude);
        town.Longitude.Should().Be(longitude);
        town.SearchString.Should().Be(searchString);
    }
}
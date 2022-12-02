using sfa.Tl.Marketing.Communication.Application.Services;
using sfa.Tl.Marketing.Communication.Tests.Common.Extensions;
using Xunit;
using FluentAssertions;
using NSubstitute;
using sfa.Tl.Marketing.Communication.Models.Dto;
using sfa.Tl.Marketing.Communication.UnitTests.Builders;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sfa.Tl.Marketing.Communication.Application.Interfaces;

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
            51.674302M,
            -1.282302M);

        ValidateTown(receivedTowns
                .SingleOrDefault(t =>
                    t.Id == 304),
            304,
            "Abingdon",
            "Inner London",
            "Greater London",
            51.497681M,
            -0.192782M);

        ValidateTown(receivedTowns
                .SingleOrDefault(t =>
                    t.Id == 72832),
            72832,
            "West Bromwich",
            "West Midlands",
            "West Midlands",
            52.530629M,
            -2.005941M);

        ValidateTown(receivedTowns
                .SingleOrDefault(t =>
                    t.Id == 72834),
            72834,
            "West Bromwich (East)",
            "West Midlands",
            "West Midlands",
            52.540693M,
            -1.942085M);

        ValidateTown(receivedTowns
                .SingleOrDefault(t =>
                    t.Id == 72835),
            72835,
            "West Bromwich Central",
            "West Midlands",
            "West Midlands",
            52.520416M,
            -1.984158M);
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
            51.674302M,
            -1.282302M);
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
            //"Sandwell",
            //"MD",
            52.530629M,
            -2.005941M);
    }

    private static void ValidateTown(Town town,
        int id,
        string name,
        string county,
        string localAuthority,
        decimal latitude,
        decimal longitude)
    {
        town.Should().NotBeNull();
        town.Id.Should().Be(id);
        town.Name.Should().Be(name);
        town.County.Should().Be(county);
        town.LocalAuthority.Should().Be(localAuthority);
        town.Latitude.Should().Be(latitude);
        town.Longitude.Should().Be(longitude);
    }
}
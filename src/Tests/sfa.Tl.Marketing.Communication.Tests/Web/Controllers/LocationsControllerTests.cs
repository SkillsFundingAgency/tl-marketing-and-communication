using sfa.Tl.Marketing.Communication.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using sfa.Tl.Marketing.Communication.Tests.Common.Extensions;
using Xunit;
using sfa.Tl.Marketing.Communication.UnitTests.Builders;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.UnitTests.Web.Controllers;

public class LocationsControllerTests
{
    [Fact]
    public void Locations_Controller_Constructor_Guards_Against_NullParameters()
    {
        typeof(LocationsController)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task Locations_Controller_SearchTowns_Returns_Expected_Value()
    {
        const string searchTerm = "Cov";
        var towns = new TownListBuilder()
            .CreateKnownList()
            .Build();

        var townDataService = Substitute.For<ITownDataService>();
        townDataService
            .Search(searchTerm, Arg.Any<int>())
            .Returns(towns);

        var controller = new LocationsControllerBuilder().Build(townDataService);

        var result = await controller.SearchLocations(searchTerm);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var results = okResult.Value as IEnumerable<Town>;
        var list = results?.ToList();
        list.Should().NotBeNullOrEmpty();
        list.Should().BeEquivalentTo(towns);
    }
}
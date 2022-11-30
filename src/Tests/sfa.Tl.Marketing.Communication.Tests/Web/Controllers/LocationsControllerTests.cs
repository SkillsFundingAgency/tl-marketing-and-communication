using sfa.Tl.Marketing.Communication.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using sfa.Tl.Marketing.Communication.Tests.Common.Extensions;
using Xunit;
using sfa.Tl.Marketing.Communication.UnitTests.Builders;
using Microsoft.AspNetCore.Mvc;
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
        var towns = new TownListBuilder().Build().ToList();

        //var townDataService = Substitute.For<TownDataService>();
        //townDataService
        //    .Search(searchTerm)
        //    .Returns(towns);   
        var controller = new LocationsControllerBuilder().Build();//townDataService);

        const string searchTerm = "Cov";

        var result = await controller.SearchLocations(searchTerm);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var results = okResult.Value as IEnumerable<Town>;
        results.Should().NotBeNullOrEmpty();
        results.Count().Should().Be(towns.Count);
        results.First().Name.Should().Be(towns[0].Name);
        results.Skip(1).First().Name.Should().Be(towns[1].Name);
        //results.Should().BeEquivalentTo(towns);
    }
}
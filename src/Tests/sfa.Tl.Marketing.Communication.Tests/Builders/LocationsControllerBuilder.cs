using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Controllers;
using sfa.Tl.Marketing.Communication.SearchPipeline;

namespace sfa.Tl.Marketing.Communication.UnitTests.Builders;

public class LocationsControllerBuilder
{
    public LocationsController Build(
        //ITownDataService townDataService = null,
        IProviderSearchEngine providerSearchEngine = null,
        ILogger<LocationsController> logger = null)
    {
        //townDataService ??= Substitute.For<ITownDataService>();
        logger ??= Substitute.For<ILogger<LocationsController>>();

        var controller = new LocationsController(
            //townDataService,
            logger)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        return controller;
    }
}
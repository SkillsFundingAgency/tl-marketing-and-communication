using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.Controllers;

[Route("api/[controller]")]
public class LocationsController : ControllerBase
{
    //private readonly ITownDataService _townDataService;
    private readonly ILogger<LocationsController> _logger;

    public LocationsController(
        //ITownDataService townDataService,
        ILogger<LocationsController> logger)
    {
        //_townDataService = townDataService ?? throw new ArgumentNullException(nameof(townDataService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    [Route("", Name = "SearchLocations")]
    [ProducesResponseType(typeof(IEnumerable<Town>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchLocations(string searchTerm)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug($"{nameof(LocationsController)} {nameof(SearchLocations)} called.");
        }

        var towns = //await _townDataService.Search(searchTerm);
            new List<Town>
            {
                new() { Name = "Coventry" }, 
                new() { Name = "Oxford" }
            };

        return Ok(towns);
    }
}
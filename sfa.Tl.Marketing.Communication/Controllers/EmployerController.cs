using System;
using Microsoft.AspNetCore.Mvc;
using sfa.Tl.Marketing.Communication.Models.Configuration;

namespace sfa.Tl.Marketing.Communication.Controllers;

public class EmployerController : Controller
{
    private readonly ConfigurationOptions _siteConfiguration;

    public EmployerController(ConfigurationOptions siteConfiguration)
    {
        _siteConfiguration = siteConfiguration ?? throw new ArgumentNullException(nameof(siteConfiguration));
    }

    // Path parameter is a catch all so all routes will be redirected
    // ReSharper disable once RouteTemplates.RouteParameterIsNotPassedToMethod
    [Route("/employers/{**path}", Name = "Employer")]
    public IActionResult EmployerHome()
    {
        return RedirectPermanent(_siteConfiguration.EmployerSupportSiteUrl);
    }

    // Path parameter is a catch all so all routes will be redirected
    // ReSharper disable once RouteTemplates.RouteParameterIsNotPassedToMethod
    [Route("/employer/{**path}")]
    public IActionResult EmployerHomeRedirect()
    {
        return RedirectPermanent(_siteConfiguration.EmployerSupportSiteUrl);
    }
}
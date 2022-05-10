using System;
using Microsoft.AspNetCore.Mvc;
using sfa.Tl.Marketing.Communication.Models.Configuration;

namespace sfa.Tl.Marketing.Communication.Controllers;

public class EmployerController : Controller
{
    private readonly EmployerSiteSettings _employerSiteSettings;

    public EmployerController(ConfigurationOptions siteConfiguration)
    {
        if(siteConfiguration is null) throw new ArgumentNullException(nameof(siteConfiguration));
        _employerSiteSettings = siteConfiguration.EmployerSiteSettings;
    }

    // Path parameter is a catch all so all routes will be redirected
    // ReSharper disable once RouteTemplates.RouteParameterIsNotPassedToMethod
    [Route("/employers/{**path}", Name = "Employers")]
    [Route("/employer/{**path}", Name = "Employer")]
    public IActionResult EmployerHome()
    {
        return RedirectPermanent(_employerSiteSettings.SiteUrl);
    }

    [Route("/employer/about", Name = "EmployerAbout")]
    [Route("/employers/about", Name = "EmployersAbout")]
    public IActionResult EmployerAbout()
    {
        var targetUrl = $"{_employerSiteSettings.SiteUrl}" +
                        $"{(_employerSiteSettings.SiteUrl.EndsWith("/") ? "" : "/")}" +
                        $"{_employerSiteSettings.AboutArticle}";

        return RedirectPermanent(targetUrl);
    }
}
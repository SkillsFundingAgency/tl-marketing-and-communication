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
    [Route("/employers/{**path}", Name = "Employers")]
    [Route("/employer/{**path}", Name = "Employer")]
    public IActionResult EmployerHome()
    {
        return RedirectPermanent(_siteConfiguration.EmployerSupportSiteUrl);
    }

    [Route("/employer/about", Name = "EmployerAbout")]
    [Route("/employers/about", Name = "EmployersAbout")]
    public IActionResult EmployerAbout()
    {
        var targetUrl = _siteConfiguration.EmployerSupportSiteUrl;
        const string aboutArticleUrlFragment = "categories/4416409666834-About-T-Levels-and-industry-placements";
        targetUrl = $"{_siteConfiguration.EmployerSupportSiteUrl}" +
                    $"{(_siteConfiguration.EmployerSupportSiteUrl.EndsWith("/") ? "" : "/")}" +
                    $"{aboutArticleUrlFragment}";

        return RedirectPermanent(targetUrl);
    }
}
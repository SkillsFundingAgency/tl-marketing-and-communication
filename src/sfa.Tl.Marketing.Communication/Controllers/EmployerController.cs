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
    [Route("/employer/{**path}", Name = "Employer")]
    [Route("/employers/{**path}", Name = "Employers")]
    public IActionResult EmployerHome()
    {
        return RedirectPermanent(_employerSiteSettings.SiteUrl);
    }

    [Route("/employer/about", Name = "EmployerAbout")]
    [Route("/employers/about", Name = "EmployersAbout")]
    public IActionResult EmployerAbout()
    {
        return RedirectPermanent(BuildTargetUrl(_employerSiteSettings.AboutArticle));
    }

    [Route("/employer/skills-available", Name = "EmployerSkills")]
    [Route("/employers/skills-available", Name = "EmployersSkills")]
    public IActionResult EmployerSkills()
    {
        return RedirectPermanent(BuildTargetUrl(_employerSiteSettings.SkillsArticle));
    }

    [Route("/employer/business-benefits", Name = "EmployerBenefits")]
    [Route("/employers/business-benefits", Name = "EmployersBenefits")]
    public IActionResult EmployerBenefits()
    {
        return RedirectPermanent(BuildTargetUrl(_employerSiteSettings.IndustryPlacementsBenefitsArticle));
    }

    [Route("/employer/what-it-costs", Name = "EmployerCostsOld")]
    [Route("/employers/what-it-costs", Name = "EmployersCostsOld")]
    [Route("/employer/how-it-works", Name = "EmployerTimeline")]
    [Route("/employers/how-it-works", Name = "EmployersTimeline")]
    public IActionResult EmployerTimeline()
    {
        return RedirectPermanent(BuildTargetUrl(_employerSiteSettings.TimelineArticle));
    }

    private string BuildTargetUrl(string articleFragment)
    {
        var targetUrl = $"{_employerSiteSettings.SiteUrl}" +
                        $"{(_employerSiteSettings.SiteUrl.EndsWith("/") ? "" : "/")}" +
                        $"{articleFragment}";
        return targetUrl;
    }
}
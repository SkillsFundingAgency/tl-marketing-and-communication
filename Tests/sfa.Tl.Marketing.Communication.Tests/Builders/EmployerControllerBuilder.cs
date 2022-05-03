using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sfa.Tl.Marketing.Communication.Controllers;
using sfa.Tl.Marketing.Communication.Models.Configuration;

namespace sfa.Tl.Marketing.Communication.UnitTests.Builders;

public class EmployerControllerBuilder
{
    public EmployerController BuildEmployerController(
        string employerSupportSiteUrl) =>
        BuildEmployerController(new ConfigurationOptions
        {
            EmployerSupportSiteUrl = employerSupportSiteUrl
        });

    public EmployerController BuildEmployerController(
        ConfigurationOptions siteConfiguration = null)
    {
        siteConfiguration ??= new ConfigurationOptions();

        var controller = new EmployerController(siteConfiguration)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        return controller;
    }
}
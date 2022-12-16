using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using NSubstitute;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Controllers;
using sfa.Tl.Marketing.Communication.SearchPipeline;

namespace sfa.Tl.Marketing.Communication.UnitTests.Builders;

public class StudentControllerBuilder
{
    public StudentController Build(
        IProviderDataService providerDataService = null,
        IProviderSearchEngine providerSearchEngine = null,
        IUrlHelper urlHelper = null,
        ISession session = null)
    {
        providerDataService ??= Substitute.For<IProviderDataService>();
        providerSearchEngine ??= Substitute.For<IProviderSearchEngine>();

        var httpContext = new DefaultHttpContext();
        if(session is not null)
        {
            httpContext.Session = session;
        }

        var controller = new StudentController(providerDataService, providerSearchEngine)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        if (urlHelper is not null)
        {
            controller.Url = urlHelper;
        }

        return controller;
    }

}
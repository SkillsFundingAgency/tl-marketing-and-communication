using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Controllers;
using sfa.Tl.Marketing.Communication.Models;
using sfa.Tl.Marketing.Communication.SearchPipeline;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Web.Controllers
{
    public class StudentControllerRedirectDataTests
    {

        [Theory]
        [InlineData("/students/something")] //Local uri
        [InlineData("https://www.solihull.ac.uk/")]
        [InlineData("https://www.ccsw.ac.uk/t%20levels/")]
        [InlineData("https://www.southessex.ac.uk/t-levels/?&level_1_gcse=true&level_2_gcse_btec=true&level_3_btec_a_level=true&a_level=true")]
        [InlineData("https://www.stokesfc.ac.uk/our-courses/?tex_post_tag=t-level&view=list")]
        public void Student_Controller_Redirect_Returns_Expected_Redirect_For_Known_Uri(string targetUri)
        {
            var providerSearchEngine = Substitute.For<IProviderSearchEngine>();

            var providerDataService = Substitute.For<IProviderDataService>();
            providerDataService
                .GetWebsiteUrls()
                .Returns(new List<string> { targetUri });

            var urlHelper = Substitute.For<IUrlHelper>();
            urlHelper.IsLocalUrl(Arg.Any<string>())
                .Returns(args => ((string)args[0]).StartsWith("/students/"));

            var controller = new StudentController(providerDataService, providerSearchEngine)
            {
                Url = urlHelper
            };

            var redirectResult = controller.Redirect(
                new RedirectViewModel
                {
                    //Use the actual url that's returned from the web site
                    Url = targetUri
                }) as RedirectResult;

            redirectResult.Should().NotBeNull();
            redirectResult?.Url.Should().Be(targetUri);
        }
    }
}

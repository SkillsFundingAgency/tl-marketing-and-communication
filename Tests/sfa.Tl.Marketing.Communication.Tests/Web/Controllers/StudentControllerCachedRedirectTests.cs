using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Controllers;
using sfa.Tl.Marketing.Communication.Models;
using sfa.Tl.Marketing.Communication.SearchPipeline;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Web.Controllers
{
    public class StudentControllerCachedRedirectTests
    {
        private const string AllowedExternalUri = "https://notevil.com";
        private const string AllowedExternalProviderUri = "https://www.abingdon-witney.ac.uk/whats-new/t-levels";
        private const string DisallowedExternalUri = "https://evil.com";

        private const string LocalUri = "/students";

        private readonly StudentController _controller;

        public StudentControllerCachedRedirectTests()
        {
            var allowedUrls = new List<string>
            {
                AllowedExternalUri,
                AllowedExternalProviderUri
            };

            var cache = Substitute.For<IMemoryCache>();
            var cachedUrls = new HashSet<string>(allowedUrls);

            cache
                .TryGetValue(StudentController.AllowedRedirectUrlsCacheKey, 
                    out Arg.Any<HashSet<string>>())
                .Returns(x => {
                    x[1] = cachedUrls;
                    return true;
                });

            var providerSearchEngine = Substitute.For<IProviderSearchEngine>();

            var providerDataService = Substitute.For<IProviderDataService>();
            providerDataService.GetWebsiteUrls().Returns(allowedUrls);

            var urlHelper = Substitute.For<IUrlHelper>();
            urlHelper.IsLocalUrl(Arg.Any<string>())
                .Returns(args => (string)args[0] == LocalUri);

            _controller = new StudentController(providerDataService, providerSearchEngine, cache)
            {
                Url = urlHelper
            };
        }

        [Fact]
        public void Student_Controller_Redirect_Returns_Expected_Redirect_For_Local_Uri()
        {
            var result = _controller.Redirect(new RedirectViewModel { Url = LocalUri });

            var redirectResult = result as RedirectResult;
            redirectResult.Should().NotBeNull();
            redirectResult?.Url.Should().Be(LocalUri);
        }

        [Fact]
        public void Student_Controller_Redirect_Returns_Expected_Redirect_For_Valid_Uri()
        {
            var result = _controller.Redirect(new RedirectViewModel { Url = AllowedExternalProviderUri });

            var redirectResult = result as RedirectResult;
            redirectResult.Should().NotBeNull();
            redirectResult?.Url.Should().Be(AllowedExternalProviderUri);
        }

        [Fact]
        public void Student_Controller_Redirect_Returns_Students_Home_For_Invalid_Uri()
        {
            var result = _controller.Redirect(new RedirectViewModel { Url = DisallowedExternalUri });

            var redirectResult = result as RedirectResult;
            redirectResult.Should().NotBeNull();
            redirectResult?.Url.Should().Be("/students");
        }
    }
}

using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sfa.Tl.Marketing.Communication.Controllers;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Web.Controllers
{
    public class EmployerControllerTests
    {
        [Fact]
        public void Employer_Controller_EmployerNextSteps_Get_Returns_Expected_Value()
        {
            var controller = BuildEmployerController();

            var result = controller.EmployerNextSteps();

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
        }

        private static EmployerController BuildEmployerController()
        {
            var controller = new EmployerController
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };

            return controller;
        }
    }
}

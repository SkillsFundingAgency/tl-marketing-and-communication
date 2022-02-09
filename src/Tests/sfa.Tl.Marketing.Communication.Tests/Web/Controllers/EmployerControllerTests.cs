using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using sfa.Tl.Marketing.Communication.UnitTests.Builders;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Web.Controllers
{
    public class EmployerControllerTests
    {
        [Fact]
        public void Employer_Controller_EmployerNextSteps_Get_Returns_Expected_Value()
        {
            var controller = new EmployerControllerBuilder().BuildEmployerController();

            var result = controller.EmployerNextSteps();

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
        }
    }
}

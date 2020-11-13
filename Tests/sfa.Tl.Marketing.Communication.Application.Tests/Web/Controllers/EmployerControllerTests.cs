using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using sfa.Tl.Marketing.Communication.Controllers;
using sfa.Tl.Marketing.Communication.Models;
using sfa.Tl.Marketing.Communication.UnitTests.Builders;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Web.Controllers
{
    public class EmployerControllerTests
    {
        //public EmployerControllerTests()
        //{
        //}

        [Fact]
        public void Employer_Controller_EmployerNextSteps_Get_Returns_Expected_Value()
        {
            var controller = new EmployerController();

            var result = controller.EmployerNextSteps();

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<ViewResult>();
        }

        [Fact]
        public void Employer_Controller_EmployerNextSteps_Post_ViewModel_Succeeds_For_Valid_Input()
        {
            var viewModel = new EmployerContactViewModelBuilder().WithDefaultValues().Build();

            var controller = new EmployerController();

            var result = controller.EmployerNextSteps(viewModel);

            result.Should().NotBeNull();
        }
        
        [Fact]
        public async Task Employer_Controller_EmployerNextSteps_Post_Validates_Full_Name()
        {
            var viewModel = new EmployerContactViewModelBuilder()
                .WithDefaultValues()
                .WithFullName(null)
                .Build();

            var controller = new EmployerController();

            var result = await controller.EmployerNextSteps(viewModel);

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<ViewResult>();

            //TODO: Get validation working
            //controller.ViewData.ModelState.IsValid.Should().BeFalse();
            //controller.ViewData.ModelState.ContainsKey(nameof(EmployerContactViewModel.FullName))
            //    .Should().BeTrue();

            //controller.ViewData.ModelState[nameof(EmployerContactViewModel.FullName)].Errors.Should().ContainSingle(error => error.ErrorMessage == "You must enter a real postcode");

            var viewResult = result as ViewResult;
            var returnedViewModel = (result as ViewResult)?.Model as EmployerContactViewModel;
            
            returnedViewModel.Should().NotBeNull();
            //viewModel?.FullName.Should().BeNullOrEmpty();
            //viewModel?.OrganisationName.Should().BeNullOrEmpty();
            //viewModel?.Email.Should().BeNullOrEmpty();
            //viewModel?.PhoneNumber.Should().BeNullOrEmpty();
            //viewModel?.ContactMethod.Should().BeNull();

        }
    }
}

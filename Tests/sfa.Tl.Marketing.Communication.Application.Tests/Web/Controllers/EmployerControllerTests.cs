using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using sfa.Tl.Marketing.Communication.Application.Enums;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Controllers;
using sfa.Tl.Marketing.Communication.Models;
using sfa.Tl.Marketing.Communication.UnitTests.Builders;
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

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<ViewResult>();
        }

        [Fact]
        public async Task Employer_Controller_EmployerNextSteps_Post_Succeeds_For_Valid_Input()
        {
            var viewModel = new EmployerContactViewModelBuilder().WithDefaultValues().Build();

            var controller = BuildEmployerController();

            var result = await controller.EmployerNextSteps(viewModel);

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task Employer_Controller_EmployerNextSteps_Post_Calls_Email_Service_For_Valid_Input()
        {
            var emailService = Substitute.For<IEmailService>();
            var controller = BuildEmployerController(emailService);
            
            var viewModel = new EmployerContactViewModelBuilder().WithDefaultValues().Build();
            await controller.EmployerNextSteps(viewModel);

            await emailService
                .Received(1)
                .SendEmployerEmail(
                    Arg.Is<string>(p => p == viewModel.FullName),
                    Arg.Is<string>(p => p == viewModel.OrganisationName),
                    Arg.Is<string>(p => p == viewModel.PhoneNumber),
                    Arg.Is<string>(p => p == viewModel.Email),
                    Arg.Is<ContactMethod>(p => p == viewModel.ContactMethod));
        }

        [Fact]
        public async Task Employer_Controller_EmployerNextSteps_Post_Validates_Full_Name()
        {
            var viewModel = new EmployerContactViewModelBuilder()
                .WithDefaultValues()
                .WithFullName(null)
                .Build();

            var controller = BuildEmployerController();

            var result = await controller.EmployerNextSteps(viewModel);

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<ViewResult>();

            //TODO: Get validation working where possible
            //controller.ViewData.ModelState.IsValid.Should().BeFalse();
            //controller.ViewData.ModelState.ContainsKey(nameof(EmployerContactViewModel.FullName))
            //    .Should().BeTrue();

            //controller.ViewData.ModelState[nameof(EmployerContactViewModel.FullName)].Errors.Should().ContainSingle(error => error.ErrorMessage == "You must enter a real postcode");

            var returnedViewModel = (result as ViewResult)?.Model as EmployerContactViewModel;
            
            returnedViewModel.Should().NotBeNull();
            //viewModel?.FullName.Should().BeNullOrEmpty();
            //viewModel?.OrganisationName.Should().BeNullOrEmpty();
            //viewModel?.Email.Should().BeNullOrEmpty();
            //viewModel?.PhoneNumber.Should().BeNullOrEmpty();
            //viewModel?.ContactMethod.Should().BeNull();
        }

        //TODO: Validate the other fields


        private EmployerController BuildEmployerController(
            IEmailService emailService = null)
        {
            emailService ??= Substitute.For<IEmailService>();
            return new EmployerController(emailService);
        }
    }
}

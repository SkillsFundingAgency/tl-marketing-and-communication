﻿using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using sfa.Tl.Marketing.Communication.Application.Enums;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Constants;
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
            var controller = BuildEmployerController();

            var result = await controller.EmployerNextSteps(new EmployerContactViewModelBuilder().WithDefaultValues().Build());

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task Employer_Controller_EmployerNextSteps_Post_Calls_Email_Service_For_Valid_Input()
        {
            var emailService = Substitute.For<IEmailService>();
            emailService.SendEmployerContactEmail(
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<ContactMethod>())
                .Returns(true);

            var controller = BuildEmployerController(emailService);
            
            var viewModel = new EmployerContactViewModelBuilder().WithDefaultValues().Build();
            await controller.EmployerNextSteps(viewModel);

            await emailService
                .Received(1)
                .SendEmployerContactEmail(
                    Arg.Is<string>(p => p == viewModel.FullName),
                    Arg.Is<string>(p => p == viewModel.OrganisationName),
                    Arg.Is<string>(p => p == viewModel.Phone),
                    Arg.Is<string>(p => p == viewModel.Email),
                    Arg.Is<ContactMethod>(p => p == viewModel.ContactMethod));
        }

        [Fact]
        public async Task Employer_Controller_EmployerNextSteps_Post_And_Send_Email_Sets_Cookie()
        {
            var emailService = Substitute.For<IEmailService>();
            emailService.SendEmployerContactEmail(
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<ContactMethod>())
                .Returns(true);

            var controller = BuildEmployerController(emailService);

            var result = await controller.EmployerNextSteps(new EmployerContactViewModelBuilder().WithDefaultValues().Build());

            var responseHeaders = controller.ControllerContext.HttpContext.Response.Headers;

            var cookieValue = "";
            foreach (var (_, value) in responseHeaders.Where(h => h.Key == "Set-Cookie"))
            {
                var header = value.FirstOrDefault(h => h.StartsWith($"{AppConstants.EmployerContactFormSentCookieName}="));
                if (header != null)
                {
                    var p1 = header.IndexOf('=');
                    var p2 = header.IndexOf(';');
                    cookieValue = header.Substring(p1 + 1, p2 - p1 - 1);
                    break;
                }
            }

            cookieValue.Should().NotBeEmpty();
            cookieValue.Should().Be("true");
            
            //https://stackoverflow.com/questions/36899875/how-can-i-check-for-a-response-cookie-in-asp-net-core-mvc-aka-asp-net-5-rc1
            //https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/testing?view=aspnetcore-5.0#unit-testing-controllers
        }
        
        [Fact]
        public async Task Employer_Controller_EmployerNextSteps_Post_Validates_Phone_With_No_Number()
        {
            var controller = BuildEmployerController();

            var result = await controller.EmployerNextSteps(
                new EmployerContactViewModelBuilder()
                    .WithDefaultValues()
                    .WithPhone("ABC")
                    .Build());

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<ViewResult>();

            controller.ViewData.ModelState.Should().ContainSingle();
            
            controller.ViewData.ModelState.ContainsKey(nameof(EmployerContactViewModel.Phone))
                .Should().BeTrue();

            var modelStateEntry =
                controller.ViewData.ModelState[nameof(EmployerContactViewModel.Phone)];
            modelStateEntry.Errors[0].ErrorMessage.Should().Be("You must enter a number");
        }

        [Fact]
        public async Task Employer_Controller_EmployerNextSteps_Post_Validates_Phone_With_Too_Few_Numbers()
        {
            var controller = BuildEmployerController();

            var result = await controller.EmployerNextSteps(
                new EmployerContactViewModelBuilder()
                    .WithDefaultValues()
                    .WithPhone("123")
                    .Build());

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<ViewResult>();

            controller.ViewData.ModelState.Should().ContainSingle();
            controller.ViewData.ModelState.ContainsKey(nameof(EmployerContactViewModel.Phone))
                .Should().BeTrue();

            var modelStateEntry =
                controller.ViewData.ModelState[nameof(EmployerContactViewModel.Phone)];
            modelStateEntry.Errors[0].ErrorMessage.Should().Be("You must enter a telephone number that has 7 or more numbers");
        }

        private EmployerController BuildEmployerController(
            IEmailService emailService = null)
        {
            emailService ??= Substitute.For<IEmailService>();

            var controller = new EmployerController(emailService)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };

            return controller;
        }
    }
}

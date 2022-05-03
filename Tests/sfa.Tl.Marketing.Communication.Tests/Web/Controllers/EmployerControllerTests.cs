﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using sfa.Tl.Marketing.Communication.Controllers;
using sfa.Tl.Marketing.Communication.UnitTests.Builders;
using sfa.Tl.Marketing.Communication.UnitTests.TestHelpers;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Web.Controllers;

public class EmployerControllerTests
{
    private const string EmployerSiteUrl = "https://test.employers.gov.uk/home";

    [Fact]
    public void Employer_Controller_Constructor_Guards_Against_NullParameters()
    {
        typeof(EmployerController)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void Employer_Controller_EmployerHome_Get_Returns_Redirect_To_Employer_Site()
    {
        var controller = new EmployerControllerBuilder()
            .BuildEmployerController(EmployerSiteUrl);

        var result = controller.EmployerHome();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult?.Permanent.Should().BeTrue();
        redirectResult?.Url.Should().Be(EmployerSiteUrl);
    }

    [Fact]
    public void Employer_Controller_EmployerHomeRedirect_Get_Returns_Redirect_To_Employer_Site()
    {
        var controller = new EmployerControllerBuilder()
            .BuildEmployerController(EmployerSiteUrl);

        var result = controller.EmployerHomeRedirect();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult?.Permanent.Should().BeTrue();
        redirectResult?.Url.Should().Be(EmployerSiteUrl);
    }
}
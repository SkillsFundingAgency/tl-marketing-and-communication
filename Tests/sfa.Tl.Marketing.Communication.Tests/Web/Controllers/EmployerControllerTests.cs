using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using sfa.Tl.Marketing.Communication.Controllers;
using sfa.Tl.Marketing.Communication.Models.Configuration;
using sfa.Tl.Marketing.Communication.UnitTests.Builders;
using sfa.Tl.Marketing.Communication.UnitTests.TestHelpers;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Web.Controllers;

public class EmployerControllerTests
{
    private const string EmployerSiteUrl = "https://test.employers.gov.uk/";
    private const string EmployerSiteAboutArticle = "about-t-levels";
    private const string EmployerSupportSiteSkillsArticle = "skill-areas";
    private const string EmployerSupportSiteIndustryPlacementsBenefitsArticle = "business-benefits";
    private const string EmployerSupportSiteTimelineArticle = "timeline";

    private const string EmployerSiteAboutPageUrl = EmployerSiteUrl + EmployerSiteAboutArticle;
    private const string EmployerSiteSkillsPageUrl = EmployerSiteUrl + EmployerSupportSiteSkillsArticle;
    private const string EmployerSiteIndustryPlacementsBenefitsPageUrl = EmployerSiteUrl + EmployerSupportSiteIndustryPlacementsBenefitsArticle;
    private const string EmployerSiteTimelinePageUrl = EmployerSiteUrl + EmployerSupportSiteTimelineArticle;

    private readonly ConfigurationOptions _employerConfigurationOptions = new()
    {
        EmployerSiteSettings = new EmployerSiteSettings
        {
            SiteUrl = EmployerSiteUrl,
            AboutArticle = EmployerSiteAboutArticle,
            IndustryPlacementsBenefitsArticle = EmployerSupportSiteIndustryPlacementsBenefitsArticle,
            SkillsArticle = EmployerSupportSiteSkillsArticle,
            TimelineArticle = EmployerSupportSiteTimelineArticle
        }
    };

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
            .BuildEmployerController(_employerConfigurationOptions);

        var result = controller.EmployerHome();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult?.Permanent.Should().BeTrue();
        redirectResult?.Url.Should().Be(EmployerSiteUrl);
    }

    [Fact]
    public void Employer_Controller_EmployerAbout_Get_Returns_Redirect_To_Employer_Site()
    {
        var controller = new EmployerControllerBuilder()
            .BuildEmployerController(_employerConfigurationOptions);

        var result = controller.EmployerAbout();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult?.Permanent.Should().BeTrue();
        redirectResult?.Url.Should().Be(EmployerSiteAboutPageUrl);
    }

    [Fact]
    public void Employer_Controller_EmployerSkills_Get_Returns_Redirect_To_Employer_Site()
    {
        var controller = new EmployerControllerBuilder()
            .BuildEmployerController(_employerConfigurationOptions);

        var result = controller.EmployerSkills();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult?.Permanent.Should().BeTrue();
        redirectResult?.Url.Should().Be(EmployerSiteSkillsPageUrl);
    }

    [Fact]
    public void Employer_Controller_EmployerBenefits_Get_Returns_Redirect_To_Employer_Site()
    {
        var controller = new EmployerControllerBuilder()
            .BuildEmployerController(_employerConfigurationOptions);

        var result = controller.EmployerBenefits();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult?.Permanent.Should().BeTrue();
        redirectResult?.Url.Should().Be(EmployerSiteIndustryPlacementsBenefitsPageUrl);
    }

    [Fact]
    public void Employer_Controller_EmployerTimeline_Get_Returns_Redirect_To_Employer_Site()
    {
        var controller = new EmployerControllerBuilder()
            .BuildEmployerController(_employerConfigurationOptions);

        var result = controller.EmployerTimeline();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult?.Permanent.Should().BeTrue();
        redirectResult?.Url.Should().Be(EmployerSiteTimelinePageUrl);
    }

}
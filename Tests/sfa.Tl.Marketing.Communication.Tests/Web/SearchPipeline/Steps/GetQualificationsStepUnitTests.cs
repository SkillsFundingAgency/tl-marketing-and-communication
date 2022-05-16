using FluentAssertions;
using NSubstitute;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models;
using sfa.Tl.Marketing.Communication.Models.Dto;
using sfa.Tl.Marketing.Communication.SearchPipeline;
using sfa.Tl.Marketing.Communication.SearchPipeline.Steps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sfa.Tl.Marketing.Communication.UnitTests.TestHelpers;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Web.SearchPipeline.Steps;

public class GetQualificationsStepUnitTests
{
    private readonly IProviderSearchService _providerSearchService;
    private readonly ISearchStep _searchStep;
    private readonly List<Qualification> _qualifications =
        new()
        {
            new() { Id = 1, Name = "Qualification 1" },
            new() { Id = 2, Name = "Qualification 2" },
            new() { Id = 3, Name = "Qualification 3" },
            new() { Id = 4, Name = "Qualification 4" },
            new() { Id = 5, Name = "Qualification 5" }
        };

    

    public GetQualificationsStepUnitTests()
    {
        _providerSearchService = Substitute.For<IProviderSearchService>();

        _providerSearchService.GetQualifications().Returns(_qualifications);

        _searchStep = new GetQualificationsStep(_providerSearchService);
    }

    [Fact]
    public void Step_Constructor_Guards_Against_NullParameters()
    {
        typeof(GetQualificationsStep)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task Step_Returns_SelectListItems_For_All_Qualifications_With_A_Selected_Qualification()
    {
        int? selectedQualificationId = 3;

        var viewModel = new FindViewModel
        {
            SelectedQualificationId = selectedQualificationId
        };

        var context = new SearchContext(viewModel);

        await _searchStep.Execute(context);

        _providerSearchService.Received(1).GetQualifications();
        context.ViewModel.Qualifications.Count().Should().Be(_qualifications.Count);
        context.ViewModel.Qualifications
            .Any(q => q.Value == selectedQualificationId.ToString() && q.Selected)
            .Should().BeTrue();

        context.ViewModel.Qualifications
            .Count(q => q.Value == selectedQualificationId.ToString() && q.Selected)
            .Should()
            .Be(1);

        var qualificationsSelectList = context.ViewModel.Qualifications.OrderBy(q => q.Value).ToList();
        qualificationsSelectList[0].Value.Should().Be("1");
        qualificationsSelectList[0].Text.Should().Be("Qualification 1");
        qualificationsSelectList[0].Selected.Should().BeFalse();

        qualificationsSelectList[1].Value.Should().Be("2");
        qualificationsSelectList[1].Text.Should().Be("Qualification 2");
        qualificationsSelectList[1].Selected.Should().BeFalse();

        qualificationsSelectList[2].Value.Should().Be("3");
        qualificationsSelectList[2].Text.Should().Be("Qualification 3");
        qualificationsSelectList[2].Selected.Should().BeTrue();

        qualificationsSelectList[3].Value.Should().Be("4");
        qualificationsSelectList[3].Text.Should().Be("Qualification 4");
        qualificationsSelectList[3].Selected.Should().BeFalse();

        qualificationsSelectList[4].Value.Should().Be("5");
        qualificationsSelectList[4].Text.Should().Be("Qualification 5");
        qualificationsSelectList[4].Selected.Should().BeFalse();

        context.ViewModel.SelectedQualificationId.Should().Be(selectedQualificationId);
    }

    [Theory]
    [InlineData(null, 0)]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(3, 3)]
    [InlineData(4, 4)]
    [InlineData(5, 5)]
    [InlineData(6, 0)] //Reset to 0 if missing id used
    [InlineData(2112, 0)] //Reset to 0 if missing id used
    public async Task Step_Sets_QualificationId_For_Search(int? qualificationId, int expected)
    {
        var viewModel = new FindViewModel
        {
            SelectedQualificationId = qualificationId
        };
        var context = new SearchContext(viewModel);

        await _searchStep.Execute(context);

        context.ViewModel.SelectedQualificationId.Should().Be(expected);
    }
}
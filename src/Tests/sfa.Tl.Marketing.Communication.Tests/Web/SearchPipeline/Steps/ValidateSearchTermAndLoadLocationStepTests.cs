using System.Linq;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Constants;
using sfa.Tl.Marketing.Communication.Models;
using sfa.Tl.Marketing.Communication.SearchPipeline;
using sfa.Tl.Marketing.Communication.SearchPipeline.Steps;
using System.Threading.Tasks;
using sfa.Tl.Marketing.Communication.Models.Dto;
using sfa.Tl.Marketing.Communication.Tests.Common.Extensions;
using sfa.Tl.Marketing.Communication.UnitTests.Builders;

namespace sfa.Tl.Marketing.Communication.UnitTests.Web.SearchPipeline.Steps;

public class ValidateSearchTermAndLoadLocationStepTests
{
    private readonly IProviderSearchService _providerSearchService;
    private readonly ITownDataService _townDataService;
    private readonly ISearchStep _searchStep;

    public ValidateSearchTermAndLoadLocationStepTests()
    {
        _providerSearchService = Substitute.For<IProviderSearchService>();
        _townDataService = Substitute.For<ITownDataService>();

        _searchStep = new ValidateSearchTermAndLoadLocationStep(_providerSearchService, _townDataService);
    }

    [Fact]
    public void Step_Constructor_Guards_Against_NullParameters()
    {
        typeof(ValidateSearchTermAndLoadLocationStep)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task Step_Validate_Empty_Postcode()
    {
        var viewModel = new FindViewModel
        {
            SearchTerm = string.Empty
        };

        var context = new SearchContext(viewModel);

        await _searchStep.Execute(context);

        context.ViewModel.ValidationStyle.Should().Be(AppConstants.ValidationStyle);
        context.ViewModel.ValidationMessage.Should().Be(AppConstants.PostcodeValidationMessage);
        context.Continue.Should().BeFalse();
    }

    [Fact]
    public async Task Step_Validates_Wrong_Postcode()
    {
        const string postcode = "CV3 9XX";
        const bool isValid = false;

        var viewModel = new FindViewModel
        {
            SearchTerm = postcode
        };

        var context = new SearchContext(viewModel);

        _providerSearchService.IsSearchPostcodeValid(postcode).Returns((isValid, null));

        await _searchStep.Execute(context);

        context.ViewModel.ValidationStyle.Should().Be(AppConstants.ValidationStyle);
        context.ViewModel.ValidationMessage.Should().Be(AppConstants.RealPostcodeOrTownValidationMessage);
        context.Continue.Should().BeFalse();
        await _providerSearchService.Received(1).IsSearchPostcodeValid(postcode);
    }

    [Fact]
    public async Task Step_Validates_Postcode()
    {
        const string postcode = "mk 4 2 8 y u";
        const string expected = "MK42 8YU";
        const double latitude = 50.0123;
        const double longitude = 1.987;
        const bool isValid = true;

        var expectedPostcodeLocation = new PostcodeLocation
        {
            Postcode = expected,
            Latitude = latitude,
            Longitude = longitude
        };

        var viewModel = new FindViewModel
        {
            SearchTerm = postcode
        };

        var context = new SearchContext(viewModel);

        _providerSearchService.IsSearchPostcodeValid(postcode).Returns((isValid, expectedPostcodeLocation));

        await _searchStep.Execute(context);

        context.ViewModel.SearchTerm.Should().Be(expected);
        context.Continue.Should().BeTrue();
        await _providerSearchService.Received(1).IsSearchPostcodeValid(postcode);
    }

    [Fact]
    public async Task Step_Validates_Valid_Town()
    {
        const string searchTerm = "Coventry";
        var town = new TownListBuilder()
            .CreateKnownList()
            .Build()
            .Single(t => t.Name == searchTerm);

        var viewModel = new FindViewModel
        {
            SearchTerm = searchTerm
        };

        var context = new SearchContext(viewModel);

        _townDataService.IsSearchTermValid(searchTerm).Returns((true, town));

        await _searchStep.Execute(context);

        context.Continue.Should().BeTrue();
        await _townDataService.Received(1).IsSearchTermValid(searchTerm);
    }

    [Fact]
    public async Task Step_Validates_Invalid_Town()
    {
        const string searchTerm = "NotCoventry";

        var viewModel = new FindViewModel
        {
            SearchTerm = searchTerm
        };

        var context = new SearchContext(viewModel);

        _townDataService.IsSearchTermValid(searchTerm).Returns((false, null));

        await _searchStep.Execute(context);

        context.Continue.Should().BeFalse();
        await _townDataService.Received(1).IsSearchTermValid(searchTerm);
    }
}
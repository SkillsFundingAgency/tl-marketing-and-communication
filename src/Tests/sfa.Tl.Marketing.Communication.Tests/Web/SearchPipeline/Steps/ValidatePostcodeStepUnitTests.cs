using System.Linq;
using FluentAssertions;
using NSubstitute;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Constants;
using sfa.Tl.Marketing.Communication.Models;
using sfa.Tl.Marketing.Communication.SearchPipeline;
using sfa.Tl.Marketing.Communication.SearchPipeline.Steps;
using System.Threading.Tasks;
using sfa.Tl.Marketing.Communication.Models.Dto;
using sfa.Tl.Marketing.Communication.Tests.Common.Extensions;
using sfa.Tl.Marketing.Communication.UnitTests.Builders;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Web.SearchPipeline.Steps;

public class ValidatePostcodeStepUnitTests
{
    private readonly IProviderSearchService _providerSearchService;
    private readonly ITownDataService _townDataService;
    private readonly ISearchStep _searchStep;

    public ValidatePostcodeStepUnitTests()
    {
        _providerSearchService = Substitute.For<IProviderSearchService>();
        _townDataService = Substitute.For<ITownDataService>();

        _searchStep = new ValidatePostcodeStep(_providerSearchService, _townDataService);
    }

    [Fact]
    public void Step_Constructor_Guards_Against_NullParameters()
    {
        typeof(ValidatePostcodeStep)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task Step_Validate_Empty_Postcode()
    {
        var viewModel = new FindViewModel
        {
            Postcode = string.Empty
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
        const string postcode = "CV1 2XX";
        const double latitude = 50.0123;
        const double longitude = 1.987;
        const bool isValid = false;

        var postcodeLocation = new PostcodeLocation
        {
            Postcode = postcode,
            Latitude = latitude,
            Longitude = longitude
        };

        var viewModel = new FindViewModel
        {
            Postcode = postcode
        };

        var context = new SearchContext(viewModel);

        _providerSearchService.IsSearchPostcodeValid(postcode).Returns((isValid, postcodeLocation));

        await _searchStep.Execute(context);

        context.ViewModel.ValidationStyle.Should().Be(AppConstants.ValidationStyle);
        context.ViewModel.ValidationMessage.Should().Be(AppConstants.RealPostcodeValidationMessage);
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
            Postcode = postcode
        };

        var context = new SearchContext(viewModel);

        _providerSearchService.IsSearchPostcodeValid(postcode).Returns((isValid, expectedPostcodeLocation));

        await _searchStep.Execute(context);

        context.ViewModel.Postcode.Should().Be(expected);
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
            Postcode = searchTerm
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
            Postcode = searchTerm
        };

        var context = new SearchContext(viewModel);

        _townDataService.IsSearchTermValid(searchTerm).Returns((false, null));

        await _searchStep.Execute(context);

        context.Continue.Should().BeFalse();
        await _townDataService.Received(1).IsSearchTermValid(searchTerm);
    }
}
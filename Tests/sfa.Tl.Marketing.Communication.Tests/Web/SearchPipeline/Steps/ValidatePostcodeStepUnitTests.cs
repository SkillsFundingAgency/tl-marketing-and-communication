using FluentAssertions;
using NSubstitute;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Constants;
using sfa.Tl.Marketing.Communication.Models;
using sfa.Tl.Marketing.Communication.SearchPipeline;
using sfa.Tl.Marketing.Communication.SearchPipeline.Steps;
using System.Threading.Tasks;
using sfa.Tl.Marketing.Communication.Models.Dto;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Web.SearchPipeline.Steps
{
    public class ValidatePostcodeStepUnitTests
    {
        private readonly IProviderSearchService _providerSearchService;
        private readonly ISearchStep _searchStep;

        public ValidatePostcodeStepUnitTests()
        {
            _providerSearchService = Substitute.For<IProviderSearchService>();
            _searchStep = new ValidatePostcodeStep(_providerSearchService);
        }

        [Fact]
        public async Task Step_Validate_Empty_Postcode()
        {
            // Arrange
            var viewModel = new FindViewModel()
            {
                Postcode = string.Empty
            };
            
            var context = new SearchContext(viewModel);

            // Act
            await _searchStep.Execute(context);

            // Assert
            context.ViewModel.ValidationStyle.Should().Be(AppConstants.ValidationStyle);
            context.ViewModel.PostcodeValidationMessage.Should().Be(AppConstants.PostcodeValidationMessage);
            context.Continue.Should().BeFalse();
        }

        [Fact]
        public async Task Step_Validate_Wrong_Postcode()
        {
            // Arrange
            const string postcode = "dddfd";
            const string latitude = "50.0123";
            const string longitude = "1.987";
            const bool isValid = false;

            var postcodeLocation = new PostcodeLocation
            {
                Postcode = postcode,
                Latitude = latitude,
                Longitude = longitude
            };

            var viewModel = new FindViewModel()
            {
                Postcode = postcode
            };

            var context = new SearchContext(viewModel);

            _providerSearchService.IsSearchPostcodeValid(Arg.Is<string>(p => p == postcode)).Returns((isValid,  postcodeLocation));

            // Act
            await _searchStep.Execute(context);

            // Assert
            context.ViewModel.ValidationStyle.Should().Be(AppConstants.ValidationStyle);
            context.ViewModel.PostcodeValidationMessage.Should().Be(AppConstants.RealPostcodeValidationMessage);
            context.Continue.Should().BeFalse();
            await _providerSearchService.Received(1).IsSearchPostcodeValid(Arg.Is<string>(p => p == postcode));
        }

        [Fact]
        public async Task Step_Validate_Postcode_And_()
        {
            // Arrange
            const string postcode = "mk 4 2 8 y u";
            const string expected = "MK42 8YU";
            const string latitude = "50.0123";
            const string longitude = "1.987";
            const bool isValid = true;

            var expectedPostcodeLocation = new PostcodeLocation
            {
                Postcode = expected,
                Latitude = latitude,
                Longitude = longitude
            };

            var viewModel = new FindViewModel()
            {
                Postcode = postcode
            };
            
            var context = new SearchContext(viewModel);

            _providerSearchService.IsSearchPostcodeValid(Arg.Is<string>(p => p == postcode)).Returns((isValid, expectedPostcodeLocation));

            // Act
            await _searchStep.Execute(context);

            // Assert
            context.ViewModel.Postcode.Should().Be(expected);
            context.Continue.Should().BeTrue();
            await _providerSearchService.Received(1).IsSearchPostcodeValid(Arg.Is<string>(p => p == postcode));
        }
    }
}

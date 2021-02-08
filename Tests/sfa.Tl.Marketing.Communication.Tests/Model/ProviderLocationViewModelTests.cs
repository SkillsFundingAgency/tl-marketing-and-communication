using FluentAssertions;
using sfa.Tl.Marketing.Communication.Models;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Model
{
    public class ProviderLocationViewModelTests
    {
        [Fact]
        public void ProviderLocationViewModel_DistanceString_Shows_Mile_When_Distance_Is_Exactly_One()
        {
            var viewModel = new ProviderLocationViewModel
            {
                DistanceInMiles = 1
            };

            viewModel.DistanceString.Should().Be("mile");
        }

        [Fact]
        public void ProviderLocationViewModel_DistanceString_Shows_Miles_When_Distance_Is_Not_Exactly_One()
        {
            var viewModel = new ProviderLocationViewModel
            {
                DistanceInMiles = 5
            };

            viewModel.DistanceString.Should().Be("miles");
        }

        [Fact]
        public void ProviderLocationViewModel_AddressLabel_Is_As_Expected_When_Venue_Name_Is_Blank()
        {
            var viewModel = new ProviderLocationViewModel
            {
                ProviderName = "Test Provider",
                Postcode = "CV1 2WT",
                Town = "Coventry",
                Name = ""
            };

            viewModel.AddressLabel.Should().Be("Coventry | CV1 2WT");
        }

        [Fact]
        public void ProviderLocationViewModel_AddressLabel_Is_As_Expected_When_Venue_Name_Is_Not_Blank()
        {
            var viewModel = new ProviderLocationViewModel
            {
                ProviderName = "Test Provider",
                Postcode = "CV1 2WT",
                Town = "Coventry",
                Name = "Venue"
            };

            viewModel.AddressLabel.Should().Be("Part of Test Provider \r\n Coventry | CV1 2WT");
        }

        [Fact]
        public void ProviderLocationViewModel_AddressLabel_Is_As_Expected_When_Venue_Name_Is_ProviderName()
        {
            var viewModel = new ProviderLocationViewModel
            {
                ProviderName = "Test Provider",
                Postcode = "CV1 2WT",
                Town = "Coventry",
                Name = "Test Provider"
            };

            viewModel.AddressLabel.Should().Be("Part of Test Provider \r\n Coventry | CV1 2WT");
        }

        [Fact]
        public void ProviderLocationViewModel_RedirectUrl_Is_As_Expected_When_Website_Is_Blank()
        {
            var viewModel = new ProviderLocationViewModel
            {
                Website = ""
            };

            viewModel.RedirectUrl.Should().BeEmpty();
        }

        [Fact]
        public void ProviderLocationViewModel_RedirectUrl_Is_As_Expected_When_Website_Is_Null()
        {
            var viewModel = new ProviderLocationViewModel
            {
                Website = null
            };

            viewModel.RedirectUrl.Should().BeEmpty();
        }

        [Fact]
        public void ProviderLocationViewModel_RedirectUrl_Is_As_Expected()
        {
            var viewModel = new ProviderLocationViewModel
            {
                Website = "https://test.com",
            };

            // ReSharper disable once StringLiteralTypo
            viewModel.RedirectUrl.Should().Be("/students/redirect?url=https%3A%2F%2Ftest.com");
        }

        [Fact]
        public void ProviderLocationViewModel_RedirectUrlLabel_Is_As_Expected()
        {
            var viewModel = new ProviderLocationViewModel
            {
                ProviderName = "Test Provider",
            };

            viewModel.RedirectUrlLabel.Should().Be("Visit Test Provider's website");
        }

        [Fact]
        public void ProviderLocationViewModel_VenueName_Shows_Name_When_Name_Exists()
        {
            var viewModel = new ProviderLocationViewModel
            {
                ProviderName = "Test Provider",
                Name = "Venue Name"
            };

            viewModel.VenueName.Should().Be("Venue Name");
        }

        [Fact]
        public void ProviderLocationViewModel_VenueName_Shows_ProviderName_When_Name_Is_Blank()
        {
            var viewModel = new ProviderLocationViewModel
            {
                ProviderName = "Test Provider",
                Name = ""
            };

            viewModel.VenueName.Should().Be("Test Provider");
        }


        [Fact]
        public void ProviderLocationViewModel_VenueName_Shows_ProviderName_When_Name_Is_Null()
        {
            var viewModel = new ProviderLocationViewModel
            {
                ProviderName = "Test Provider",
                Name = null
            };

            viewModel.VenueName.Should().Be("Test Provider");
        }
    }
}

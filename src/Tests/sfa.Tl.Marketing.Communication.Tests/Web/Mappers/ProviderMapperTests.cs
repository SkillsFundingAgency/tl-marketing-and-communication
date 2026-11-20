using System.Collections.Generic;
using System.Linq;
using Mapster;
using MapsterMapper;
using sfa.Tl.Marketing.Communication.Models.Dto;
using sfa.Tl.Marketing.Communication.Mappers;
using sfa.Tl.Marketing.Communication.Models;
using sfa.Tl.Marketing.Communication.UnitTests.Builders;

namespace sfa.Tl.Marketing.Communication.UnitTests.Web.Mappers;

public class ProviderMapperTests
{
    [Fact]
    public void ProviderMapper_Implements_All_Properties()
    {
        var config = CreateConfig();

        var compile = () => config.Compile();

        compile.Should().NotThrow();
    }

    [Fact]
    public void ProviderLocation_With_Null_DeliveryYears_Is_Mapped_To_Empty_List()
    {
        var providerLocation = new ProviderLocation { DeliveryYears = null };

        var viewModel = CreateMapper().Map<ProviderLocationViewModel>(providerLocation);

        viewModel.DeliveryYears.Should().NotBeNull();
        viewModel.DeliveryYears.Should().BeEmpty();
    }

    [Fact]
    public void DeliveryYear_With_Null_Qualifications_Is_Mapped_To_Empty_List()
    {
        var deliveryYear = new DeliveryYear { Year = 2020, Qualifications = null };

        var viewModel = CreateMapper().Map<DeliveryYearViewModel>(deliveryYear);

        viewModel.Qualifications.Should().NotBeNull();
        viewModel.Qualifications.Should().BeEmpty();
    }

    [Theory]
    [InlineData(2.5, 3)]
    [InlineData(3.5, 4)]
    [InlineData(2.4, 2)]
    [InlineData(2.6, 3)]
    public void ProviderLocation_DistanceInMiles_Is_Rounded_Away_From_Zero(double distance, int expected)
    {
        var providerLocation = new ProviderLocation { DistanceInMiles = distance };

        var viewModel = CreateMapper().Map<ProviderLocationViewModel>(providerLocation);

        viewModel.DistanceInMiles.Should().Be(expected);
    }

    [Fact]
    public void Ignored_Members_Are_Not_Mapped()
    {
        var mapper = CreateMapper();

        var providerLocationViewModel = mapper.Map<ProviderLocationViewModel>(new ProviderLocationBuilder().Build());
        var deliveryYearViewModel = mapper.Map<DeliveryYearViewModel>(new DeliveryYearBuilder().Build());

        providerLocationViewModel.HasFocus.Should().BeFalse();
        deliveryYearViewModel.IsAvailableNow.Should().BeFalse();
    }

    private static TypeAdapterConfig CreateConfig()
    {
        var config = new TypeAdapterConfig();
        new ProviderMapper().Register(config);
        return config;
    }

    private static IMapper CreateMapper() => new Mapper(CreateConfig());

    [Fact]
    public void ProviderLocation_Is_Mapped_To_ProviderLocationViewModel_Correctly()
    {
        var providerLocation = new ProviderLocationBuilder().Build();

        var mapper = CreateMapper();

        var viewModel = mapper.Map<ProviderLocationViewModel>(providerLocation);

        viewModel.Should().NotBeNull();
        viewModel.ProviderName.Should().Be("Test Provider");
        viewModel.Name.Should().Be("Test Location");
        viewModel.Postcode.Should().Be("CV1 2WT");
        viewModel.Town.Should().Be("Coventry");
        viewModel.Latitude.Should().Be(52.400997);
        viewModel.Longitude.Should().Be(-1.508122);
        viewModel.DistanceInMiles.Should().Be(10);
        viewModel.DeliveryYears.Should().NotBeNull();
        viewModel.Website.Should().Be("https://test.provider.co.uk");
    }

    [Fact]
    public void ProviderLocationList_Is_Mapped_To_ProviderLocationViewModelList_Correctly()
    {
        var providerLocationList = new ProviderLocationListBuilder()
            .Add(2)
            .Build();

        var mapper = CreateMapper();

        var viewModelList = mapper.Map<IEnumerable<ProviderLocationViewModel>>(providerLocationList).ToList();

        viewModelList.Should().NotBeNull();
        viewModelList.Should().NotBeEmpty();
        viewModelList.Count.Should().Be(2);

        viewModelList[0].ProviderName.Should().Be("Test Provider 1");
        viewModelList[0].Name.Should().Be("Test Location 1");
        viewModelList[0].Postcode.Should().Be("CV1 2WT");
        viewModelList[0].Town.Should().Be("Coventry");
        viewModelList[0].Latitude.Should().Be(52.400997);
        viewModelList[0].Longitude.Should().Be(-1.508122);
        viewModelList[0].DistanceInMiles.Should().Be(10);
        viewModelList[0].Website.Should().Be("https://test.provider.co.uk");

        viewModelList[0].DeliveryYears.Should().NotBeNull();
        viewModelList[0].DeliveryYears.Count().Should().Be(1);
        viewModelList[0].DeliveryYears.First().Year.Should().Be(2021);
        viewModelList[0].DeliveryYears.First().Qualifications.Should().NotBeNull();
        viewModelList[0].DeliveryYears.First().Qualifications.Count.Should().Be(1);
        viewModelList[0].DeliveryYears.First().Qualifications.First().Id.Should().Be(1);
        viewModelList[0].DeliveryYears.First().Qualifications.First().Name.Should().Be("Qualification 1");


        viewModelList[1].ProviderName.Should().Be("Test Provider 2");
        viewModelList[1].Name.Should().Be("Test Location 2");
        viewModelList[1].Postcode.Should().Be("CV2 3WT");
        viewModelList[1].Town.Should().Be("Coventry");
        viewModelList[1].Latitude.Should().Be(53.400997);
        viewModelList[1].Longitude.Should().Be(-2.508122);
        viewModelList[1].DistanceInMiles.Should().Be(11);
        viewModelList[1].Website.Should().Be("https://test.provider.co.uk");

        viewModelList[1].DeliveryYears.Should().NotBeNull();
        viewModelList[1].DeliveryYears.Count().Should().Be(1);
        viewModelList[1].DeliveryYears.First().Year.Should().Be(2022);
        viewModelList[1].DeliveryYears.First().Qualifications.Should().NotBeNull();
        viewModelList[1].DeliveryYears.First().Qualifications.Count.Should().Be(1);
        viewModelList[1].DeliveryYears.First().Qualifications.First().Id.Should().Be(2);
        viewModelList[1].DeliveryYears.First().Qualifications.First().Name.Should().Be("Qualification 2");
    }

    [Fact]
    public void Qualification_Is_Mapped_To_QualificationViewModel_Correctly()
    {
        var qualification = new QualificationListBuilder()
            .Add()
            .Build()
            .First();

        var mapper = CreateMapper();

        var viewModel = mapper.Map<QualificationViewModel>(qualification);

        viewModel.Should().NotBeNull();
        viewModel.Name.Should().Be("Test Qualification 1");
        viewModel.Id.Should().Be(1);
    }

    [Fact]
    public void DeliveryYear_Is_Mapped_To_DeliveryYearViewModel_Correctly()
    {
        var deliveryYear = new DeliveryYearBuilder().Build();

        var mapper = CreateMapper();

        var viewModel = mapper.Map<DeliveryYearViewModel>(deliveryYear);

        viewModel.Should().NotBeNull();
        viewModel.Year.Should().Be(2020);
        viewModel.Qualifications.Should().NotBeNullOrEmpty();

        viewModel.Qualifications.First().Name.Should().Be("Test Qualification");
        viewModel.Qualifications.First().Id.Should().Be(1);
    }
}
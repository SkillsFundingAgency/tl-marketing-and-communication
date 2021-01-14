using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Application.Services;
using sfa.Tl.Marketing.Communication.Models.Dto;
using sfa.Tl.Marketing.Communication.Models.Entities;
using sfa.Tl.Marketing.Communication.UnitTests.Builders;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests
{
    public class TableStorageServiceTests
    {
        [Fact]
        public async Task TableStorageService_RetrieveProviders_Returns_Expected_Results()
        {
            var service = BuildTableStorageService();

            var providers = new ProviderListBuilder()
                .Add()
                .Build();

            var result = await service.RetrieveProviders();

            //TODO: Implement method and put back check below
            //result.Should().BeEquivalentTo(providers);
        }

        [Fact]
        public async Task TableStorageService_SaveProviders_Returns_Expected_Count_Of_Items_Saved()
        {
            var service = BuildTableStorageService();

            var providers = new ProviderListBuilder()
                .Add(3)
                .Build();
            var result = await service.SaveProviders(providers);

            //TODO: Implement method and put back check below
            //result.Should().Be(providers.Count);
        }

        [Fact]
        public async Task TableStorageService_RetrieveQualifications_Returns_Expected_Results()
        {
            var repository = Substitute.For<ICloudTableRepository<QualificationEntity>>();
            repository
                .GetAll()
                .Returns(new QualificationEntityListBuilder()
                    .Add()
                    .Build());

            var service = BuildTableStorageService(qualificationRepository: repository);

            var qualifications = new QualificationListBuilder()
                .Add()
                .Build();
            var result = await service.RetrieveQualifications();

            result.Should().BeEquivalentTo(qualifications);
        }

        [Fact]
        public async Task TableStorageService_SaveQualifications_With_Null_Qualifications_List_Returns_Zero()
        {
            var service = BuildTableStorageService();

            var result = await service.SaveQualifications(null);
            result.Should().Be(0);
        }

        [Fact]
        public async Task TableStorageService_SaveQualifications_With_Empty_Qualifications_List_Returns_Zero()
        {
            var service = BuildTableStorageService();

            var result = await service.SaveQualifications(new List<Qualification>());
            result.Should().Be(0);
        }

        [Fact]
        public async Task TableStorageService_SaveQualifications_Returns_Expected_Count_Of_Items_Saved()
        {
            var savedQualificationEntities = new List<QualificationEntity>();

            var qualifications = new QualificationListBuilder()
                .Add(2)
                .Build();

            var repository = Substitute.For<ICloudTableRepository<QualificationEntity>>();
            repository
                .Save(Arg.Do<IList<QualificationEntity>>(entities =>
                {
                    savedQualificationEntities.AddRange(entities);
                }))
                .Returns(qualifications.Count);

            var service = BuildTableStorageService(qualificationRepository: repository);

            var result = await service.SaveQualifications(qualifications);

            result.Should().Be(qualifications.Count);
            result.Should().Be(2);

            savedQualificationEntities.Count.Should().Be(qualifications.Count);
        }

        private TableStorageService BuildTableStorageService(
            ICloudTableRepository<ProviderEntity> providerRepository = null,
            ICloudTableRepository<QualificationEntity> qualificationRepository = null,
            ILogger<TableStorageService> logger = null)
        {
            providerRepository ??= Substitute.For<ICloudTableRepository<ProviderEntity>>();
            qualificationRepository ??= Substitute.For<ICloudTableRepository<QualificationEntity>>();
            logger ??= Substitute.For<ILogger<TableStorageService>>();

            return new TableStorageService(providerRepository, qualificationRepository, logger);
        }
    }
}
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using sfa.Tl.Marketing.Communication.Data.Entities;
using sfa.Tl.Marketing.Communication.Data.Interfaces;
using sfa.Tl.Marketing.Communication.Data.Services;
using sfa.Tl.Marketing.Communication.Data.UnitTests.Builders;
using Xunit;

namespace sfa.Tl.Marketing.Communication.Data.UnitTests
{
    public class TableStorageServiceTests
    {
        [Fact]
        public async Task TableStorageServiceTests_()
        {
            var service = BuildTableStorageService();

            //var result = await service.();

            //result.Should().Be(1);
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
using System.Net.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Application.Services;

namespace sfa.Tl.Marketing.Communication.UnitTests.Builders;

internal class TownDataServiceBuilder
{
    public ITownDataService Build(
        HttpClient httpClient = null,
        //ITownRepository townRepository = null,
        ILogger<TownDataService> logger = null)
    {
        httpClient ??= Substitute.For<HttpClient>();
        //townRepository ??= Substitute.For<ITownRepository>();
        logger ??= Substitute.For<ILogger<TownDataService>>();

        return new TownDataService(
            httpClient,
            //townRepository,
            logger);
    }
}
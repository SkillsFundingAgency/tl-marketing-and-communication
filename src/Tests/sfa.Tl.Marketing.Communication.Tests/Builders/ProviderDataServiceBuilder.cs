using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Application.Services;
using sfa.Tl.Marketing.Communication.Models.Configuration;
using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.UnitTests.Builders;

public class ProviderDataServiceBuilder
{
    public static IProviderDataService CreateProviderDataService(
        ITableStorageService tableStorageService = null,
        IMemoryCache cache = null,
        ConfigurationOptions configuration = null)
    {
        tableStorageService ??= Substitute.For<ITableStorageService>();
        cache ??= Substitute.For<IMemoryCache>();
        configuration ??= new ConfigurationOptions
        {
            CacheExpiryInSeconds = 1
        };

        return new ProviderDataService(tableStorageService, cache, configuration);
    }

    public static IProviderDataService CreateProviderDataService(
        IList<Provider> providers,
        IList<Qualification> qualifications)
    {
        var tableStorageService = Substitute.For<ITableStorageService>();
        tableStorageService.GetAllProviders().Returns(providers);
        tableStorageService.GetAllQualifications().Returns(qualifications);
        return CreateProviderDataService(tableStorageService);
    }
}
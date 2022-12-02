﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Application.Services;
using sfa.Tl.Marketing.Communication.Tests.Common.HttpClientHelpers;

namespace sfa.Tl.Marketing.Communication.UnitTests.Builders;

internal class TownDataServiceBuilder
{
    public ITownDataService Build(
        HttpClient httpClient = null,
        ITableStorageService tableStorageService = null,
        ILogger<TownDataService> logger = null)
    {
        httpClient ??= Substitute.For<HttpClient>();
        tableStorageService ??= Substitute.For<ITableStorageService>();
        logger ??= Substitute.For<ILogger<TownDataService>>();

        return new TownDataService(
            httpClient,
            tableStorageService,
            logger);
    }

    public ITownDataService Build(
        IDictionary<string, string> responseMessages,
        ITableStorageService tableStorageService = null,
        ILogger<TownDataService> logger = null)
    {
        var responsesWithUri = responseMessages
            .ToDictionary(
                item => new Uri(item.Key),
                item => item.Value);

        var httpClient = new TestHttpClientFactory()
            .CreateHttpClientWithBaseUri(
                new Uri(TownDataService.OfficeForNationalStatisticsLocationUrl),
                responsesWithUri.First().Key.ToString(),
            responsesWithUri.First().Value);

        return Build(
            httpClient,
            tableStorageService,
            logger);
    }
}
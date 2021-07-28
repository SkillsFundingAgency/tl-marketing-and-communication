using System;
using System.Collections.Generic;
using System.IO;
using AutoMapper;
using BenchmarkDotNet.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using sfa.Tl.Marketing.Communication.Application.GeoLocations;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Application.Services;
using sfa.Tl.Marketing.Communication.Controllers;
using sfa.Tl.Marketing.Communication.Mappers;
using sfa.Tl.Marketing.Communication.Models;
using sfa.Tl.Marketing.Communication.Models.Configuration;
using sfa.Tl.Marketing.Communication.Models.Dto;
using sfa.Tl.Marketing.Communication.SearchPipeline;

namespace sfa.Tl.Marketing.Communication.Benchmarks
{
    [MinColumn]
    [MaxColumn]
    public class StudentControllerBenchmarks
    {
        private readonly StudentController _studentController;
        
        private readonly FindViewModel _findViewModel = new()
        {
            Postcode = "CV1 2WT",
            NumberOfItemsToShow = 5,
            SelectedQualificationId = 0
        };

        private readonly RedirectViewModel _redirectViewModel = new()
        {
            Url = "https://www.runshaw.ac.uk/"
        };

        public StudentControllerBenchmarks()
        {
            var configuration = Helpers.BuildConfiguration();

            var tableStorageConnectionString = configuration.GetValue<string>("TableStorageConnectionString");
           
            var loggerFactory = new LoggerFactory();

            var tableStorageService = Helpers.CreateTableStorageService(tableStorageConnectionString, loggerFactory);

            var providers = tableStorageService.GetAllProviders().GetAwaiter().GetResult();
            var qualifications = tableStorageService.GetAllQualifications().GetAwaiter().GetResult();

            var cache = Substitute.For<IMemoryCache>();
            cache
                .TryGetValue<IList<Qualification>>("Qualification_Table_Data", out _)
                .Returns(callInfo =>
                {
                    callInfo[1] = qualifications;
                    return true;
                });

            cache
                .TryGetValue<IList<Provider>>("Provider_Table_Data", out _)
                .Returns(callInfo =>
                {
                    callInfo[1] = providers;
                    return true;
                });

            IProviderDataService providerDataService = new ProviderDataService(
                Substitute.For<ITableStorageService>(),
                cache,
                new ConfigurationOptions
                {
                    CacheExpiryInSeconds = 3600
                });

            var journeyService = new JourneyService();

            var locationApiClient = Substitute.For<ILocationApiClient>();
            locationApiClient
                .GetGeoLocationDataAsync(Arg.Any<string>())
                .Returns(new PostcodeLookupResultDto
                {
                    Postcode = "CV1 2WT",
                    Latitude = 52.400997,
                    Longitude = -1.508122
                });

            var distanceCalculationService = new DistanceCalculationService(
                locationApiClient
            );

            IProviderSearchService providerSearchService = new ProviderSearchService(
                providerDataService,
                journeyService,
                distanceCalculationService,
                Substitute.For<ILogger<ProviderSearchService>>());

            ISearchPipelineFactory searchPipelineFactory = new SearchPipelineFactory();

            IMapper mapper = new Mapper(new MapperConfiguration(c => { c.AddMaps(typeof(ProviderMapper).Assembly); }));

            IProviderSearchEngine providerSearchEngine = new ProviderSearchEngine(
                providerSearchService,
                mapper,
                searchPipelineFactory
            );

            var urlHelper = Substitute.For<IUrlHelper>();
            urlHelper.IsLocalUrl(Arg.Any<string>())
                .Returns(args => ((string)args[0]).StartsWith("/students/"));

            var logger = Substitute.For<ILogger<StudentController>>();

            _studentController = new StudentController(providerDataService, providerSearchEngine, logger)
            {
                Url = urlHelper
            };
        }
        
        [Benchmark(Description = "Find")]
        public void FindBenchmark() => Find();

        public IActionResult Find()
        {
            var results = _studentController.Find(_findViewModel).GetAwaiter().GetResult();
            return results;
        }

        [Benchmark(Description = "Redirect")]
        public void RedirectBenchmark() => Redirect();

        public IActionResult Redirect()
        {
            var result = _studentController.Redirect(_redirectViewModel);
            return result;
        }
    }
}

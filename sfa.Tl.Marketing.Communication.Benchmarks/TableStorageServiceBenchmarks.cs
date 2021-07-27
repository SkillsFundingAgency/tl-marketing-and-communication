using System.Collections.Generic;
using System.IO;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models.Dto;

namespace sfa.Tl.Marketing.Communication.Benchmarks
{
    [MinColumn]
    [MaxColumn]
    public class TableStorageServiceBenchmarks
    {
        private readonly ITableStorageService _tableStorageService;

        public TableStorageServiceBenchmarks()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true)
                .AddJsonFile("appsettings.development.json", true);

            var configuration = builder.Build();

            var tableStorageConnectionString = configuration.GetValue<string>("TableStorageConnectionString");

            var loggerFactory = new LoggerFactory();

            _tableStorageService = Helpers.CreateTableStorageService(tableStorageConnectionString, loggerFactory);
        }

        [Benchmark(Description = "Load providers")]
        public void LoadProvidersBenchmark() => LoadProviders();

        public IList<Provider> LoadProviders()
        {
            var providers = _tableStorageService.GetAllProviders().GetAwaiter().GetResult();
            return providers;
        }

        [Benchmark(Description = "Load qualifications")]
        public void LoadQualificationsBenchmark() => LoadQualifications();

        public IList<Qualification> LoadQualifications()
        {
            var qualifications = _tableStorageService.GetAllQualifications().GetAwaiter().GetResult();
            return qualifications;
        }
    }
}

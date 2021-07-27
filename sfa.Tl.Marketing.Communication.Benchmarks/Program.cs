using BenchmarkDotNet.Running;
using sfa.Tl.Marketing.Communication.Benchmarks;

//var tableStorageBenchmarks = new TableStorageServiceBenchmarks();
//var qualifications = tableStorageBenchmarks.LoadQualifications();
//var providers = tableStorageBenchmarks.LoadProviders();

//var controllerBenchmarks = new StudentControllerBenchmarks();
//var searchResults = controllerBenchmarks.Find();
//var redirectResults = controllerBenchmarks.Redirect();

BenchmarkRunner.Run<StudentControllerBenchmarks>();

BenchmarkRunner.Run<TableStorageServiceBenchmarks>();

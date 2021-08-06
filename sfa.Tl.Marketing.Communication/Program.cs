using System.Reflection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using sfa.Tl.Marketing.Communication;

CreateWebHostBuilder(args).Build().Run();

static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
    WebHost.CreateDefaultBuilder(args)
        .ConfigureLogging(logging =>
        {
            logging.AddDebug();
            logging.AddConsole();

            logging.AddApplicationInsights(@"APPINSIGHTS_INSTRUMENTATIONKEY");
            
            var programTypeName = MethodBase.GetCurrentMethod()?.DeclaringType?.FullName;
            if (programTypeName != null)
            {
                logging.AddFilter<Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider>(
                    programTypeName, LogLevel.Trace);
            }

            logging.AddFilter<Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider>
                (typeof(Startup).FullName, LogLevel.Trace);
        })
        .UseStartup<Startup>();

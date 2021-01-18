using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace sfa.Tl.Marketing.Communication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.AddDebug();
                    logging.AddConsole();

                    logging.AddApplicationInsights(@"APPINSIGHTS_INSTRUMENTATIONKEY");
                    logging.AddFilter<Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider>
                        (typeof(Program).FullName, LogLevel.Trace);
                    logging.AddFilter<Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider>
                        (typeof(Startup).FullName, LogLevel.Trace);
                })
                .UseStartup<Startup>();
    }
}

using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using sfa.Tl.Marketing.Communication.Functions;

[assembly: FunctionsStartup(typeof(Startup))]
namespace sfa.Tl.Marketing.Communication.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
        }
    }
}

using BadmintonLt.Integration.Players.Crawler;
using BadmintonLt.Integration.Players.Crawler.Dependencies;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Watts.Shared.Functions.DependencyInjection.Composition.Configuration;

[assembly: WebJobsStartup(typeof(Startup))]
namespace BadmintonLt.Integration.Players.Crawler
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.UseServiceProvider<AutofacScopedServiceProvider>();
        }
    }
}
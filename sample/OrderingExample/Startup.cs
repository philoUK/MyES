using OrderingExample.DI;

[assembly: Microsoft.Azure.WebJobs.Hosting.WebJobsStartup(typeof(OrderingExample.Startup))]
namespace OrderingExample
{
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Hosting;

    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddExtension<InjectConfiguration>();
        }
    }
}

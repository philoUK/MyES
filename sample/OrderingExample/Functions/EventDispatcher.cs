namespace OrderingExample.Functions
{
    using System.Threading.Tasks;
    using Attributes;
    using DI;
    using Helpers;
    using Microsoft.Azure.WebJobs;
    using Persistence.Azure;
    using Serilog;

    public static class EventDispatcher
    {
        [FunctionName("EventDispatcher")]
        public static async Task Run(
            [QueueTrigger("events", Connection = "AzureStorage")]EventPublishedNotification myQueueItem,
            [Logger(Function="EventDispatcher")] ILogger log,
            [Inject]IHandlerFactory factory)
        {
            log.Information("Dispatching event {@MyQueueItem}", myQueueItem);
            await factory.Dispatch(myQueueItem);
        }
    }
}

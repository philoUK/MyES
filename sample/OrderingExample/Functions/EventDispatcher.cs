namespace OrderingExample.Functions
{
    using System.Threading.Tasks;
    using DI;
    using Helpers;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using Persistence.Azure;

    public static class EventDispatcher
    {
        [FunctionName("EventDispatcher")]
        public static async Task Run(
            [QueueTrigger("events", Connection = "AzureStorage")]EventPublishedNotification myQueueItem,
            ILogger log,
            [Inject]IHandlerFactory factory)
        {
            await factory.Dispatch(myQueueItem);
        }
    }
}

namespace OrderingExample.Functions
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Logging;

    public static class CancelOrder
    {
        [FunctionName("CancelOrder")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [OrchestrationClient] DurableOrchestrationClient client,
            ILogger log)
        {
            string instanceId = req.Query["instanceId"];

            log.LogInformation($"Going to cancel an order for instance id {instanceId}");

            await client.RaiseEventAsync(instanceId, ExternalEvents.OrderCancelled);

            return new OkObjectResult("Sorry to see you leave");
        }
    }
}

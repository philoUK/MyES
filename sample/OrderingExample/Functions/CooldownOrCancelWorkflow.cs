namespace OrderingExample.Functions
{
    using System.Threading;
    using System.Threading.Tasks;
    using DI;
    using Domain.Events;
    using DurableFunctionExtensions;
    using MediatR;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;

    public static class CooldownOrCancelWorkflow
    {
        [FunctionName("BeginCooldownOrCancel")]
        public static async Task RunOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context,
            ILogger log)
        {
            var @event = context.GetInput<OrderPlaced>();
            var instanceId = context.InstanceId;
            log.LogInformation($"Starting the cooldown workflow for Order Id {@event.OrderId} and instanceId {instanceId}");

            // send the customer an "email" asking them to respond with the Cancel message
            // the instance id is dead important -- this will allow the right instance of this workflow
            // to pick up the data.
            log.LogInformation($"Pretending to send customer email to cancel order {instanceId}");

            await WaitForEventOrTimeout(context, log, @event);
        }

        private static async Task WaitForEventOrTimeout(
            DurableOrchestrationContext context,
            ILogger log,
            OrderPlaced @event)
        {
            using (var timeoutCs = new CancellationTokenSource())
            {
                var waitForCancel = context.WaitForExternalEvent(ExternalEvents.OrderCancelled);

                var waitForTimeout = context.CreateLongRunningTimer(@event.CooldownPeriodExpires, timeoutCs.Token);

                log.LogInformation("Waiting for either a cooldown to expire OR the customer to cancel");

                Task winner = await Task.WhenAny(waitForCancel, waitForTimeout);
                if (winner == waitForCancel)
                {
                    await context.CallActivityAsync("CancelOrder_Activity", @event);
                }
                else if (winner == waitForTimeout)
                {
                    await context.CallActivityAsync("ProvisionOrder_Activity", @event);
                }

                if (!waitForTimeout.IsCompleted)
                {
                    // All pending timers must be complete or cancelled before the function exits
                    timeoutCs.Cancel();
                }
            }
        }

        [FunctionName("CancelOrder_Activity")]
        public static async Task CancelOrder(
            [ActivityTrigger] OrderPlaced @event,
            [Inject] IMediator mediator,
            ILogger log)
        {
            // This should be a command that gets raised, and not the implementation, else we risk it happening more
            // than one time
            await mediator.Send(new Application.MediatrHandlers.CancelOrder.Command(@event.OrderId));
        }

        [FunctionName("ProvisionOrder_Activity")]
        public static async Task ProvisionOrder(
            [ActivityTrigger] OrderPlaced @event,
            [Inject] IMediator mediator,
            ILogger log)
        {
            await mediator.Send(new Application.MediatrHandlers.ProvisionOrder.Command(@event.OrderId));
        }

        [FunctionName("Start")]
        public static async Task Run(
            [QueueTrigger("cooldowns", Connection = "AzureStorage")]
            OrderPlaced @event,
            [OrchestrationClient] DurableOrchestrationClient starter,
            ILogger log)
        {
            log.LogInformation("Received an OrderPlaced event on the queeue -- starting a new workflow now");
            await starter.StartNewAsync("BeginCooldownOrCancel", @event);
        }
    }
}
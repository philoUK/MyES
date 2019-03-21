using DurableFunctionExtensions;
using Microsoft.Build.Framework;

namespace OrderingExample.Functions
{
    using System.Threading;
    using System.Threading.Tasks;
    using DI;
    using Domain.Entities;
    using Domain.Events;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using Persistence;

    public static class CooldownOrCancelWorkflow
    {
        [FunctionName("BeginCooldownOrCancel")]
        public static async Task RunOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context,
            [Inject] IAggregateRepository repository,
            ILogger log)
        {
            var @event = context.GetInput<OrderPlaced>();
            var instanceId = context.InstanceId;
            log.LogInformation($"Starting the cooldown workflow for Order Id {@event.OrderId} and instanceId {instanceId}");

            // send the customer an "email" asking them to respond with the Cancel message
            // the instance id is dead important -- this will allow the right instance of this workflow
            // to pick up the data.
            log.LogInformation($"Pretending to send customer email to cancel order {instanceId}");

            await WaitForEventOrTimeout(context, repository, log, @event);
        }

        private static async Task WaitForEventOrTimeout(
            DurableOrchestrationContext context,
            IAggregateRepository repository,
            ILogger log,
            OrderPlaced @event)
        {
            using (var timeoutCs = new CancellationTokenSource())
            {
                var waitForCancel = context.WaitForExternalEvent(InternalEvents.OrderCancelled);

                var waitForTimeout = context.CreateLongRunningTimer(@event.CooldownPeriodExpires, timeoutCs.Token);

                log.LogInformation("Waiting for either a cooldown to expire OR the customer to cancel");

                Task winner = await Task.WhenAny(waitForCancel, waitForTimeout);
                if (winner == waitForCancel)
                {
                    await CancelOrder(repository, log, @event);
                }
                else if (winner == waitForTimeout)
                {
                    await ProvisionOrder(repository, log, @event);
                }

                if (!waitForTimeout.IsCompleted)
                {
                    // All pending timers must be complete or cancelled before the function exits
                    timeoutCs.Cancel();
                }
            }
        }

        private static async Task CancelOrder(IAggregateRepository repository, ILogger log, OrderPlaced @event)
        {
            var order = await repository.Load<Order>(@event.OrderId);
            log.LogInformation("The customer cancelled");
            if (order != null)
            {
                order.Cancel("Customer requested cancellation");
                await repository.Save(order);
            }
        }

        private static async Task ProvisionOrder(IAggregateRepository repository, ILogger log, OrderPlaced @event)
        {
            log.LogInformation($"The cooldown expired, so provisioning order {@event.OrderId} now");
            var order = await repository.Load<Order>(@event.OrderId);

            // order is ready to go, cooling period expired
            if (order != null)
            {
                order.Provision();
                await repository.Save(order);
            }
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
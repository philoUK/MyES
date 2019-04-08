namespace OrderingExample.Functions
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Attributes;
    using DI;
    using Domain.Events;
    using DurableFunctionExtensions;
    using MediatR;
    using MediatRExtensions;
    using Microsoft.Azure.WebJobs;
    using Serilog;

    public static class CooldownOrCancelWorkflow
    {
        [FunctionName("BeginCooldownOrCancel")]
        public static async Task RunOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context,
            [Logger(Function = "BeginCooldownOrCancel")] ILogger log)
        {
            var @event = context.GetInput<OrderPlaced>();
            var instanceId = context.InstanceId;
            if (!context.IsReplaying)
            {
                log.Information(
                    "Starting the cooldown workflow for Order Id {OrderId} and instanceId {InstanceId}",
                    @event.OrderId,
                    instanceId);
            }

            // send the customer an "email" asking them to respond with the Cancel message
            // the instance id is dead important -- this will allow the right instance of this workflow
            // to pick up the data.
            log.Information("Pretending to send customer email to cancel order {InstanceId}", instanceId);

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

                if (!context.IsReplaying)
                {
                    log.Information("Waiting for either a cooldown to expire OR the customer to cancel");
                }

                Task winner = await Task.WhenAny(waitForCancel, waitForTimeout);
                var standardRetryOptions = new RetryOptions(TimeSpan.FromSeconds(5), 5);

                if (winner == waitForCancel)
                {
                    log.Information("Order {OrderId} was cancelled", @event.OrderId);
                    await context.CallActivityWithRetryAsync("CancelOrder_Activity", standardRetryOptions, @event);
                }
                else if (winner == waitForTimeout)
                {
                    log.Information("Order {OrderId} has cooled down and will be provisioned", @event.OrderId);
                    await context.CallActivityWithRetryAsync("ProvisionOrder_Activity", standardRetryOptions, @event);
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
            [Logger(Function = "CancelOrder_Activity")] ILogger log)
        {
            log.Information("Cancelling order {OrderId}", @event.OrderId);
            var innerCmd = new Application.MediatrHandlers.CancelOrder.Command(@event.OrderId);
            var outerCmd = new QueuedWrapper.Command(innerCmd);
            await mediator.Send(outerCmd);
        }

        [FunctionName("ProvisionOrder_Activity")]
        public static async Task ProvisionOrder(
            [ActivityTrigger] OrderPlaced @event,
            [Inject] IMediator mediator,
            [Logger(Function = "ProvisionOrder_Activity")] ILogger log)
        {
            log.Information("Provisioning order {OrderId}", @event.OrderId);
            var innerCmd = new Application.MediatrHandlers.ProvisionOrder.Command(@event.OrderId);
            var outerCmd = new QueuedWrapper.Command(innerCmd);
            await mediator.Send(outerCmd);
        }

        [FunctionName("Start")]
        public static async Task Run(
            [QueueTrigger("cooldowns", Connection = "AzureStorage")]
            OrderPlaced @event,
            [OrchestrationClient] DurableOrchestrationClient starter,
            [Logger(Function = "Start")] Serilog.ILogger log)
        {
            log.Information("Received an OrderPlaced event on the queeue -- starting a new workflow now");
            await starter.StartNewAsync("BeginCooldownOrCancel", @event);
        }
    }
}
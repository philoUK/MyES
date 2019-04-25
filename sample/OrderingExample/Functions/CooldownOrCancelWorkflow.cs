namespace OrderingExample.Functions
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Attributes;
    using Config;
    using DI;
    using Domain.Events;
    using DurableFunctionExtensions;
    using Extensions;
    using MediatR;
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

            if (!context.IsReplaying)
            {
                log.Information("Pretending to send customer email to cancel order {InstanceId}", instanceId);
            }

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
                    await context.CallActivityWithRetryAsync("CancelOrder_Activity", standardRetryOptions, @event);
                }
                else if (winner == waitForTimeout)
                {
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
            await innerCmd.SendViaMessageQueue(mediator);
        }

        [FunctionName("ProvisionOrder_Activity")]
        public static async Task ProvisionOrder(
            [ActivityTrigger] OrderPlaced @event,
            [Inject] IMediator mediator,
            [Logger(Function = "ProvisionOrder_Activity")] ILogger log)
        {
            log.Information("Provisioning order {OrderId}", @event.OrderId);
            var innerCmd = new Application.MediatrHandlers.ProvisionOrder.Command(@event.OrderId);
            await innerCmd.SendViaMessageQueue(mediator);
        }

        [FunctionName("Start")]
        public static async Task Run(
            [QueueTrigger(QueueNames.Cooldowns, Connection = "AzureStorage")]
            OrderPlaced @event,
            [OrchestrationClient] DurableOrchestrationClient starter,
            [Logger(Function = "Start")] Serilog.ILogger log)
        {
            log.Information("Received an OrderPlaced event on the queeue -- starting a new workflow now");
            await starter.StartNewAsync("BeginCooldownOrCancel", @event);
        }
    }
}
namespace OrderingExample.Functions
{
    using System;
    using System.Threading.Tasks;
    using MediatR;
    using MediatRExtensions;
    using Microsoft.Azure.WebJobs;
    using Newtonsoft.Json;
    using OrderingExample.Attributes;
    using OrderingExample.DI;

    public static class QueuedCommandDispatcher
    {
        [FunctionName("QueuedCommandDispatcher")]
        public static async Task Run(
            [QueueTrigger("commands", Connection = "AzureStorage")]QueuedWrapper.Command cmd,
            [Logger(Function = "QueuedCommandDispatcher")] Serilog.ILogger log,
            [Inject]IMediator mediator)
        {
            var innerCmd = JsonConvert.DeserializeObject(cmd.WrappedJson, Type.GetType(cmd.WrappedType));
            await mediator.Send((IRequest)innerCmd);
        }
    }
}

namespace OrderingExample.Functions
{
    using System;
    using System.Threading.Tasks;
    using Attributes;
    using Config;
    using DI;
    using MediatR;
    using MediatRExtensions;
    using Microsoft.Azure.WebJobs;
    using Newtonsoft.Json;

    public static class QueuedCommandDispatcher
    {
        [FunctionName("QueuedCommandDispatcher")]
        public static async Task Run(
            [QueueTrigger(QueueNames.Commands, Connection = "AzureStorage")]QueuedWrapper.Command cmd,
            [Logger(Function = "QueuedCommandDispatcher")] Serilog.ILogger log,
            [Inject]IMediator mediator)
        {
            var innerCmd = JsonConvert.DeserializeObject(cmd.WrappedJson, Type.GetType(cmd.WrappedType));
            await mediator.Send((IRequest)innerCmd);
        }
    }
}

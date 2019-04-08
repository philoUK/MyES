namespace OrderingExample.Functions
{
    using System;
    using System.Threading.Tasks;
    using MediatR;
    using MediatRExtensions;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using OrderingExample.DI;

    public static class QueuedCommandDispatcher
    {
        [FunctionName("QueuedCommandDispatcher")]
        public static async Task Run(
            [QueueTrigger("queuedcommands", Connection = "AzureStorage")]QueuedWrapper.Command cmd,
            ILogger log,
            [Inject]IMediator mediator)
        {
            var innerCmdType = Type.GetType(cmd.WrappedType);
            var innerCmd = JsonConvert.DeserializeObject(cmd.WrappedJson, innerCmdType);
            await mediator.Send((IRequest)innerCmd);
        }
    }
}

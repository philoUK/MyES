namespace OrderingExample.Extensions
{
    using System.Threading.Tasks;
    using MediatR;
    using MediatRExtensions;

    public static class RequestExtensions
    {
        public static async Task SendViaMessageQueue(this IRequest request, IMediator mediator)
        {
            var outerCmd = new QueuedWrapper.Command(request);
            await mediator.Send(outerCmd);
        }
    }
}

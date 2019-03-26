namespace OrderingExample.Application.AggregateEventHandlers
{
    using System.Threading.Tasks;
    using Core;
    using Domain.Events;

    public class KickOffCooldownWorkflowHandler : IHandleAggregateEventsOf<OrderPlaced>
    {
        private readonly ICooldownWorkflowHandler handler;

        public KickOffCooldownWorkflowHandler(ICooldownWorkflowHandler handler)
        {
            this.handler = handler;
        }

        public void Handle(OrderPlaced @event)
        {
            this.HandleAsync(@event).Wait();
        }

        public async Task HandleAsync(OrderPlaced @event)
        {
            await this.handler.Handle(@event);
        }
    }
}

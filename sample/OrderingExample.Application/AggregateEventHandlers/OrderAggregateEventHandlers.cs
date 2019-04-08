namespace OrderingExample.Application.AggregateEventHandlers
{
    using System.Threading.Tasks;
    using Core;
    using Domain.Events;
    using ReadModels;

    public class OrderAggregateEventHandlers : IHandleAggregateEventsOf<OrderPlaced>
    {
        private readonly IOrderReadModel readModel;

        public OrderAggregateEventHandlers(IOrderReadModel readModel)
        {
            this.readModel = readModel;
        }

        public async Task HandleAsync(OrderPlaced @event)
        {
            await this.readModel.RecordNewOrder(@event.OrderId, @event.CooldownPeriodExpires);
        }
    }
}

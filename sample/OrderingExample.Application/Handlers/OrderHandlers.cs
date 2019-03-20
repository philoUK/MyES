namespace OrderingExample.Application.Handlers
{
    using System.Threading.Tasks;
    using Core;
    using Domain.Events;
    using ReadModels;

    public class OrderHandlers : IHandleAggregateEventsOf<OrderPlaced>, IHandleAggregateEventsOf<OrderCancelled>
    {
        private readonly IOrderReadModel readModel;

        public OrderHandlers(IOrderReadModel readModel)
        {
            this.readModel = readModel;
        }

        public async Task HandleAsync(OrderPlaced @event)
        {
            await this.readModel.RecordNewOrder(@event.OrderId, @event.CooldownPeriodExpires);
        }

        public async Task HandleAsync(OrderCancelled @event)
        {
            await this.readModel.RemoveOrder(@event.OrderId);
        }
    }
}

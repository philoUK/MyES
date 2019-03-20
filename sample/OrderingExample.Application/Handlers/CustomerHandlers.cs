namespace OrderingExample.Application.Handlers
{
    using System.Threading.Tasks;
    using Core;
    using Domain.Events;
    using ReadModels;

    public class CustomerHandlers : IHandleAggregateEventsOf<OrderPlaced>, IHandleAggregateEventsOf<OrderProvisioned>, IHandleAggregateEventsOf<OrderCancelled>
    {
        private readonly ICustomerReadModel readModel;

        public CustomerHandlers(ICustomerReadModel readModel)
        {
            this.readModel = readModel;
        }

        public async Task HandleAsync(OrderPlaced @event)
        {
            await this.readModel.RecordOrderForCustomer(@event.CustomerId, @event.CooldownPeriodExpires);
        }

        public async Task HandleAsync(OrderProvisioned @event)
        {
            await this.readModel.RemoveOrderForCustomer(@event.CustomerId.Value);
        }

        public async Task HandleAsync(OrderCancelled @event)
        {
            await this.readModel.RemoveOrderForCustomer(@event.CustomerId);
        }
    }
}

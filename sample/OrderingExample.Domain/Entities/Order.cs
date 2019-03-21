namespace OrderingExample.Domain.Entities
{
    using System;
    using Core;
    using Events;
    using ValueTypes;

    public sealed class Order : Aggregate, IDispatchAggregateEventsOf<OrderPlaced>, IDispatchAggregateEventsOf<OrderCancelled>,
        IDispatchAggregateEventsOf<OrderProvisioned>
    {
        private CustomerId customerId;
        private OrderNumber orderNumber;
        private bool isPlaced;
        private DateTime? coolDownPeriodEnds;
        private bool isCancelled;
        private string cancellationReason;
        private bool isProvisioned;

        public Order()
            : base(new TextIdStrategy())
        {
        }

        public void Place(CustomerId custId, OrderNumber orderId)
        {
            if (this.isPlaced)
            {
                throw new InvalidOperationException("Order already placed");
            }

            var @event = new OrderPlaced
            {
                CooldownPeriodExpires = DateTime.UtcNow.AddDays(6),
                CustomerId = custId.Value,
                OrderId = orderId.Value
            };

            this.ApplyWithCustomId(@event, @event.OrderId);
        }

        void IDispatchAggregateEventsOf<OrderPlaced>.Handle(OrderPlaced @event)
        {
            this.customerId = new CustomerId(@event.CustomerId);
            this.orderNumber = new OrderNumber(@event.OrderId);
            this.coolDownPeriodEnds = @event.CooldownPeriodExpires;
            this.isPlaced = true;
        }

        public void Cancel(string reason)
        {
            if (this.isCancelled)
            {
                throw new InvalidOperationException("Order already cancelled");
            }

            var @event = new OrderCancelled
            {
                CustomerId = this.customerId.Value,
                OrderId = this.orderNumber.Value,
                Reason = reason
            };
            this.Apply(@event);
        }

        void IDispatchAggregateEventsOf<OrderCancelled>.Handle(OrderCancelled @event)
        {
            this.isCancelled = true;
            this.cancellationReason = @event.Reason;
        }

        public void Provision()
        {
            if (!this.isPlaced)
            {
                throw new InvalidOperationException("Order not placed");
            }

            if (this.isCancelled)
            {
                throw new InvalidOperationException("Order has been cancelled");
            }

            if (this.isProvisioned)
            {
                throw new InvalidOperationException("Order has been provisioned");
            }

            this.Apply(new OrderProvisioned { OrderId = this.orderNumber.Value, CustomerId = this.customerId });
        }

        void IDispatchAggregateEventsOf<OrderProvisioned>.Handle(OrderProvisioned @event)
        {
            this.isProvisioned = true;
            this.coolDownPeriodEnds = null;
        }
    }
}

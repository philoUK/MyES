namespace OrderingExample.Domain.Events
{
    using Core;
    using ValueTypes;

    public class OrderProvisioned : AggregateEvent
    {
        public string OrderId { get; set; }

        public CustomerId CustomerId { get; set; }
    }
}

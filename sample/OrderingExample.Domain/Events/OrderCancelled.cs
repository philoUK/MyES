namespace OrderingExample.Domain.Events
{
    using Core;

    public class OrderCancelled : AggregateEvent
    {
        public string CustomerId { get; set; }

        public string OrderId { get; set; }

        public string Reason { get; set; }
    }
}

namespace OrderingExample.Domain.Events
{
    using System;
    using Core;

    public class OrderPlaced : AggregateEvent
    {
        public string CustomerId { get; set; }

        public string OrderId { get; set; }

        public DateTime CooldownPeriodExpires { get; set; }
    }
}

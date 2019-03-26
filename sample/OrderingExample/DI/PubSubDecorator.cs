using OrderingExample.Application.AggregateEventHandlers;

namespace OrderingExample.DI
{
    using System;
    using System.Collections.Generic;
    using Core;
    using Domain.Events;
    using Persistence.Azure;

    public class PubSubDecorator : IEventSubscriberRegistry
    {
        private readonly IEventSubscriberRegistry registry;

        public PubSubDecorator(IEventSubscriberRegistry registry)
        {
            this.registry = registry;
            this.registry.SubscribeToEvent(typeof(OrderPlaced), typeof(CustomerAggregateEventHandlers));
            this.registry.SubscribeToEvent(typeof(OrderPlaced), typeof(OrderAggregateEventHandlers));
            this.registry.SubscribeToEvent(typeof(OrderPlaced), typeof(KickOffCooldownWorkflowHandler));
            this.registry.SubscribeToEvent(typeof(OrderCancelled), typeof(OrderAggregateEventHandlers));
            this.registry.SubscribeToEvent(typeof(OrderCancelled), typeof(CustomerAggregateEventHandlers));
            this.registry.SubscribeToEvent(typeof(OrderProvisioned), typeof(CustomerAggregateEventHandlers));
        }

        public IEnumerable<Type> GetSubscribersForEvent(IAggregateEvent @event)
        {
            return this.registry.GetSubscribersForEvent(@event);
        }

        public void SubscribeToEvent(Type eventType, Type subscriberType)
        {
            this.registry.SubscribeToEvent(eventType, subscriberType);
        }
    }
}

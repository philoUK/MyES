namespace OrderingExample.DI
{
    using System;
    using System.Collections.Generic;
    using Application.Handlers;
    using Core;
    using Domain.Events;
    using Persistence.Azure;

    public class PubSubDecorator : IEventSubscriberRegistry
    {
        private readonly IEventSubscriberRegistry registry;

        public PubSubDecorator(IEventSubscriberRegistry registry)
        {
            this.registry = registry;
            this.registry.SubscribeToEvent(typeof(OrderPlaced), typeof(CustomerHandlers));
            this.registry.SubscribeToEvent(typeof(OrderPlaced), typeof(OrderHandlers));
            this.registry.SubscribeToEvent(typeof(OrderPlaced), typeof(KickOffCooldownWorkflowHandler));
            this.registry.SubscribeToEvent(typeof(OrderCancelled), typeof(OrderHandlers));
            this.registry.SubscribeToEvent(typeof(OrderCancelled), typeof(CustomerHandlers));
            this.registry.SubscribeToEvent(typeof(OrderProvisioned), typeof(CustomerHandlers));
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

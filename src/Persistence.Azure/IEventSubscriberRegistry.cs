namespace Persistence.Azure
{
    using System;
    using System.Collections.Generic;
    using Core;

    public interface IEventSubscriberRegistry
    {
        IEnumerable<Type> GetSubscribersForEvent(IAggregateEvent @event);

        void SubscribeToEvent(Type eventType, Type subscriberType);
    }
}
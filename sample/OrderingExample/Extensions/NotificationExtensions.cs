namespace OrderingExample.Extensions
{
    using System;
    using Core;
    using Newtonsoft.Json;
    using Persistence.Azure;

    public static class NotificationExtensions
    {
        public static IAggregateEvent ToEvent(this EventPublishedNotification notification)
        {
            var eventType = Type.GetType(notification.EventType);
            return (IAggregateEvent)JsonConvert.DeserializeObject(notification.EventJson, eventType);
        }
    }
}

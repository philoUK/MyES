namespace Persistence.Azure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using Newtonsoft.Json;

    internal static class AggregateEventExtensions
    {
        public static LatestVersionEntity ToLatestVersionEntity(this IAggregateEvent @event)
        {
            return new LatestVersionEntity
            {
                PartitionKey = @event.Id,
                RowKey = string.Empty,
                Version = @event.Version
            };
        }

        private static string ToEventFormat(int version)
        {
            return version.ToString().PadLeft(5, '0');
        }

        public static EventEntity ToEventEntity(this IAggregateEvent @event)
        {
            return new EventEntity
            {
                PartitionKey = @event.Id,
                RowKey = ToEventFormat(@event.Version),
                EventJson = JsonConvert.SerializeObject(@event),
                EventName = @event.GetType().Name,
                EventType = @event.GetType().AssemblyQualifiedName
            };
        }

        public static IAggregateEvent ToEvent(this EventEntity entity)
        {
            var eventType = Type.GetType(entity.EventType);
            return (IAggregateEvent)JsonConvert.DeserializeObject(entity.EventJson, eventType);
        }

        public static IEnumerable<IEnumerable<T>> ToBatch<T>(this IEnumerable<T> source, int size)
        {
            var currentCount = 0;
            var currentBatch = new List<T>();
            foreach (var element in source)
            {
                currentBatch.Add(element);
                currentCount++;
                if (currentCount == size)
                {
                    yield return currentBatch.ToList();
                    currentCount = 0;
                    currentBatch = new List<T>();
                }
            }

            if (currentBatch.Any())
            {
                yield return currentBatch.ToList();
            }
        }

        public static EventPublishedNotification ToEventPublishedNotification(this IAggregateEvent @event, Type subscriberType)
        {
            return new EventPublishedNotification
            {
                EventJson = JsonConvert.SerializeObject(@event),
                EventType = @event.GetType().AssemblyQualifiedName,
                SubscriberType = subscriberType.AssemblyQualifiedName
            };
        }
    }
}

namespace Core
{
    using System.Collections.Generic;

    public interface IAggregate
    {
        int Version { get; }

        string Id { get; }

        void Load(IEnumerable<IAggregateEvent> events);

        IEnumerable<IAggregateEvent> PendingEvents();
    }
}
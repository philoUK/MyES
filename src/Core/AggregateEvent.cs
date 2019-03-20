namespace Core
{
    using System;

    public abstract class AggregateEvent : IAggregateEvent
    {
        public int Version { get; set; }

        public string Id { get; set; }

        public DateTime DateOfEvent { get; set; }
    }
}

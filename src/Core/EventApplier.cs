namespace Core
{
    using System;

    internal class EventApplier
    {
        private readonly Aggregate aggregate;

        public EventApplier(Aggregate aggregate)
        {
            this.aggregate = aggregate;
        }

        public void Execute(IAggregateEvent @event)
        {
            this.PrepareEvent(@event);
            this.AddToPendingEvents(@event);
            this.InvokeReducer(@event);
        }

        private void PrepareEvent(IAggregateEvent @event)
        {
            @event.Version = this.aggregate.Version + 1;
            @event.Id = this.aggregate.Id;
            @event.DateOfEvent = DateTime.UtcNow;
        }

        private void AddToPendingEvents(IAggregateEvent @event)
        {
            this.aggregate.events.Add(@event);
        }

        internal void InvokeReducer(IAggregateEvent @event)
        {
            this.aggregate.Version = @event.Version;
            this.aggregate.ConsumeDispatchedEvent(@event);
        }
    }
}

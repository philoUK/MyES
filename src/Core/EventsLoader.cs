namespace Core
{
    using System.Collections.Generic;
    using System.Linq;

    internal class EventsLoader
    {
        private readonly Aggregate aggregate;

        public EventsLoader(Aggregate aggregate)
        {
            this.aggregate = aggregate;
        }

        public void Execute(IEnumerable<IAggregateEvent> events)
        {
            var nextExpectedId = this.aggregate.Version + 1;

            foreach (var @event in events.OrderBy(e => e.Version))
            {
                this.LoadEvent(@event, nextExpectedId);
                nextExpectedId++;
            }
        }

        private void LoadEvent(IAggregateEvent @event, int nextExpectedId)
        {
            this.EnsureEventIsTheExpectedVersion(@event, nextExpectedId);
            this.EnsureEventIsForTheCorrectAggregate(nextExpectedId, @event);
            this.SetId(nextExpectedId, @event);
            this.InvokeReducer(@event);
        }

        private void EnsureEventIsTheExpectedVersion(IAggregateEvent @event, int nextExpectedId)
        {
            if (@event.Version != nextExpectedId)
            {
                throw new UnexpectedEventException(
                    $"Aggregate {this.aggregate.GetType().Name} expected to load an event of version {nextExpectedId} but was {@event.Version}");
            }
        }

        private void SetId(int nextExpectedId, IAggregateEvent @event)
        {
            if (nextExpectedId == 1)
            {
                this.aggregate.idStrategy.SetId(@event.Id);
            }
        }

        private void EnsureEventIsForTheCorrectAggregate(int nextExpectedId, IAggregateEvent @event)
        {
            if (nextExpectedId > 1 && @event.Id != this.aggregate.Id)
            {
                throw new UnexpectedEventException(
                    $"Aggregate {this.aggregate.GetType().Name} expected to load an event of Id {this.aggregate.Id} but received an event of type {@event.GetType().Name} with an id of {@event.Id}");
            }
        }

        private void InvokeReducer(IAggregateEvent @event)
        {
            this.aggregate.Version = @event.Version;
            this.aggregate.ConsumeDispatchedEvent(@event);
        }
    }
}
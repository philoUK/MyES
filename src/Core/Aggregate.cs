namespace Core
{
    using System;
    using System.Collections.Generic;

    public abstract class Aggregate : IAggregate
    {
        internal readonly List<IAggregateEvent> events;
        internal readonly IdentityStrategy idStrategy;

        protected Aggregate()
        : this(new GuidIdentityStrategy())
        {
        }

        protected Aggregate(IdentityStrategy idStrategy)
        {
            this.events = new List<IAggregateEvent>();
            this.Version = 0;
            this.idStrategy = idStrategy;
        }

        public int Version { get; internal set; }

        public string Id => this.idStrategy.GetId();

        public void Load(IEnumerable<IAggregateEvent> events)
        {
            var loader = new EventsLoader(this);
            loader.Execute(events);
        }

        public IEnumerable<IAggregateEvent> PendingEvents() => this.events;

        public override bool Equals(object obj)
        {
            if (!(obj is Aggregate rhs))
            {
                return false;
            }

            if (rhs.GetType() != this.GetType())
            {
                return false;
            }

            if (object.ReferenceEquals(this, rhs))
            {
                return true;
            }

            return rhs.Id == this.Id && rhs.Version == this.Version;
        }

        public override int GetHashCode()
        {
            if (this.Version > 0)
            {
                return this.Id.GetHashCode(StringComparison.InvariantCultureIgnoreCase);
            }

            return this.GetType().Name.GetHashCode(StringComparison.InvariantCultureIgnoreCase);
        }

        protected void Apply(IAggregateEvent @event)
        {
            var applier = new EventApplier(this);
            applier.Execute(@event);
        }

        protected void ApplyWithCustomId(IAggregateEvent @event, string customId)
        {
            this.idStrategy.SetId(customId);
            this.Apply(@event);
        }
    }
}

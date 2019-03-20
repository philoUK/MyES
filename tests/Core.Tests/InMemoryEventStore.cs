namespace Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Persistence;

    internal class InMemoryEventStore : IAggregateEventStore
    {
        private readonly List<IAggregateEvent> events;
        private bool returnOutOfRangeLatestVersion = false;

        public InMemoryEventStore()
        {
            this.events = new List<IAggregateEvent>();
        }

        public int EventCount => this.events.Count;

        public Task<int> GetLatestVersionOf(string aggregateId)
        {
            if (this.returnOutOfRangeLatestVersion)
            {
                return Task.FromResult(int.MaxValue);
            }

            var latest = this.events.Where(e => e.Id.Equals(aggregateId, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(e => e.Version).FirstOrDefault();

            return latest == null ? Task.FromResult(0) : Task.FromResult(latest.Version);
        }

        public Task Store(IEnumerable<IAggregateEvent> events)
        {
            this.events.AddRange(events);
            return Task.CompletedTask;
        }

        public void GiveIncorrectLatestVersion()
        {
            this.returnOutOfRangeLatestVersion = true;
        }


        public Task<IEnumerable<IAggregateEvent>> GetEventsForId(string id)
        {
            var results = this.events.Where(e => e.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(results);
        }
    }
}

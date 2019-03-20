namespace Persistence
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core;

    public class AggregateRepository : IAggregateRepository
    {
        private readonly IAggregateEventStore eventStore;
        private readonly IAggregateEventPublisher eventPublisher;

        public AggregateRepository(IAggregateEventStore eventStore, IAggregateEventPublisher eventPublisher)
        {
            this.eventStore = eventStore;
            this.eventPublisher = eventPublisher;
        }

        public async Task Save(IAggregate aggregate)
        {
            var events = aggregate.PendingEvents().OrderBy(e => e.Version).ToList();

            if (events.Any())
            {
                await this.EnsureNoConcurrencyProblems(aggregate, events);
                await this.StoreAllEvents(events);
                await this.PublishAllEvents(events);
            }
        }

        private async Task EnsureNoConcurrencyProblems(IAggregate aggregate, List<IAggregateEvent> events)
        {
            var latestVersion = await this.eventStore.GetLatestVersionOf(aggregate.Id);
            var firstVersion = events.First().Version;

            if (latestVersion != (firstVersion - 1))
            {
                throw new EventStoreConcurrencyException(
                    $"Aggregate of type {aggregate.GetType().Name} with an Id of {aggregate.Id} already has an event with that same version.  Reload the aggregate and retry the operation");
            }
        }

        private async Task StoreAllEvents(List<IAggregateEvent> events)
        {
            await this.eventStore.Store(events);
        }

        private async Task PublishAllEvents(List<IAggregateEvent> events)
        {
            await this.eventPublisher.Publish(events);
        }

        public async Task<T> Load<T>(string id)
            where T : IAggregate, new()
        {
            var events = await this.eventStore.GetEventsForId(id);
            T result = new T();
            result.Load(events.OrderBy(e => e.Version));
            return result.Version == 0 ? default(T) : result;
        }
    }
}

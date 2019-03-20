namespace Persistence
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core;

    public interface IAggregateEventStore
    {
        Task<int> GetLatestVersionOf(string aggregateId);

        Task Store(IEnumerable<IAggregateEvent> events);

        Task<IEnumerable<IAggregateEvent>> GetEventsForId(string id);
    }
}

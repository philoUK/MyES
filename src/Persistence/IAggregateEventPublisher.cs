namespace Persistence
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core;

    public interface IAggregateEventPublisher
    {
        Task Publish(IEnumerable<IAggregateEvent> @events);
    }
}
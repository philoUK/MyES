namespace Core
{
    public interface IDispatchAggregateEventsOf<in T>
        where T : IAggregateEvent
    {
        void Handle(T message);
    }
}

namespace Core
{
    using System.Threading.Tasks;

    /// <summary>
    /// SUGGESTION:  This interface should *ALWAYS* be explicitly implemented as it should only ever be called
    /// by the infrastructure
    /// </summary>
    /// <typeparam name="T">Anything really</typeparam>
    public interface IHandleAggregateEventsOf<in T>
        where T : IAggregateEvent
    {
        Task HandleAsync(T @event);
    }
}

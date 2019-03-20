namespace Core
{
    using System.Globalization;
    using System.Reflection;
    using System.Threading.Tasks;

    public static class EventHandlingExtensions
    {
        public static void ConsumeDispatchedEvent(this IAggregate aggregate, IAggregateEvent @event)
        {
            var type = typeof(IDispatchAggregateEventsOf<>);
            var eventType = @event.GetType();
            var fullType = type.MakeGenericType(eventType);
            fullType.InvokeMember(
                "Handle",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod,
                null,
                aggregate,
                new object[] { @event },
                CultureInfo.InvariantCulture);
        }

        public static async Task ConsumeSubscribedEvent(this object subscriber, IAggregateEvent @event)
        {
            var type = typeof(IHandleAggregateEventsOf<>);
            var eventType = @event.GetType();
            var fullType = type.MakeGenericType(eventType);
            await (Task)fullType.InvokeMember(
                "HandleAsync",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod,
                null,
                subscriber,
                new object[] { @event },
                CultureInfo.InvariantCulture);
        }
    }
}

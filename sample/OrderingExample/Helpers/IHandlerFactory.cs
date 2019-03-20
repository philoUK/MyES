namespace OrderingExample.Helpers
{
    using System.Threading.Tasks;
    using Persistence.Azure;

    public interface IHandlerFactory
    {
        Task Dispatch(EventPublishedNotification notification);
    }
}

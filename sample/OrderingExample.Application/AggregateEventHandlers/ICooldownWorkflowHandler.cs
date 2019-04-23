namespace OrderingExample.Application.AggregateEventHandlers
{
    using System.Threading.Tasks;
    using Domain.Events;

    public interface ICooldownWorkflowHandler
    {
        Task Handle(OrderPlaced message);
    }
}
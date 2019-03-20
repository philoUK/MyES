namespace OrderingExample.Application.Handlers
{
    using System.Threading.Tasks;
    using Domain.Events;

    public interface ICooldownWorkflowHandler
    {
        Task Handle(OrderPlaced @event);
    }
}
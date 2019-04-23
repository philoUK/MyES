namespace OrderingExample.Azure
{
    using System.Threading.Tasks;
    using Application.AggregateEventHandlers;
    using Domain.Events;
    using Helpers;

    public class QueueCooldownWorkflowHandler : ICooldownWorkflowHandler
    {
        private readonly QueueCooldownWorkflowHandlerConfig config;

        public QueueCooldownWorkflowHandler(QueueCooldownWorkflowHandlerConfig config)
        {
            this.config = config;
        }

        public async Task Handle(OrderPlaced message)
        {
            var forwarder = new EventForwarder(this.config.ConnectionString, this.config.QueueName);
            await forwarder.Handle(message);
        }
    }
}

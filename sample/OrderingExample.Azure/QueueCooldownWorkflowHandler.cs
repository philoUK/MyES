﻿namespace OrderingExample.Azure
{
    using System.Threading.Tasks;
    using Application.Handlers;
    using Domain.Events;
    using Helpers;

    public class QueueCooldownWorkflowHandler : ICooldownWorkflowHandler
    {
        private readonly QueueCooldownWorkflowHandlerConfig config;

        public QueueCooldownWorkflowHandler(QueueCooldownWorkflowHandlerConfig config)
        {
            this.config = config;
        }

        public async Task Handle(OrderPlaced @event)
        {
            var forwarder = new EventForwarder(this.config.ConnectionString, this.config.QueueName);
            await forwarder.Handle(@event);
        }
    }
}

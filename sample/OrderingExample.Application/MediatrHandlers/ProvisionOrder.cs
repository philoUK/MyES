﻿using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OrderingExample.Domain.Entities;
using Persistence;

namespace OrderingExample.Application.MediatrHandlers
{
    public static class ProvisionOrder
    {
        public class Command : IRequest
        {
            public Command(string orderId)
            {
                this.OrderId = orderId;
            }

            public string OrderId { get; }
        }

        public class Handler : AsyncRequestHandler<Command>
        {
            private readonly IAggregateRepository repository;

            public Handler(IAggregateRepository repository)
            {
                this.repository = repository;
            }

            protected override async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var order = await this.repository.Load<Order>(request.OrderId);
                if (order != null)
                {
                    await this.repository.Save(order);
                }
            }
        }
    }
}

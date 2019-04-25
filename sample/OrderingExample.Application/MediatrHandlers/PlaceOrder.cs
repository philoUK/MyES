namespace OrderingExample.Application.MediatrHandlers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Domain.Entities;
    using Domain.ValueTypes;
    using MediatR;
    using Persistence;
    using Services;
    using Validators;

    public static class PlaceOrder
    {
        public class Command : IRequest
        {
            public Command(OrderNumber orderNumber, CustomerId customerId)
            {
                this.OrderNumber = orderNumber;
                this.CustomerId = customerId;
            }

            public OrderNumber OrderNumber { get; }

            public CustomerId CustomerId { get; }
        }

        public class Handler : AsyncRequestHandler<Command>
        {
            private readonly IAggregateRepository repository;
            private readonly IOrderHistory orderHistory;
            private readonly ICustomerHistory customerHistory;

            public Handler(IAggregateRepository repository, IOrderHistory orderHistory, ICustomerHistory customerHistory)
            {
                this.repository = repository;
                this.orderHistory = orderHistory;
                this.customerHistory = customerHistory;
            }

            protected override async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var validator = new PlaceOrderValidator();
                var result = await validator.Validate(request.OrderNumber, request.CustomerId, this.orderHistory, this.customerHistory);
                if (!result.HasPassed)
                {
                    throw new InvalidOperationException($"PlaceOrder failed with the following reasons: {result.ErrorList()}");
                }

                var order = new Order();
                order.Place(request.CustomerId, request.OrderNumber);
                await this.repository.Save(order);
            }
        }
    }
}

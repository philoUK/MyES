namespace OrderingExample.Application.Validators
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.ValueTypes;
    using Services;

    public class PlaceOrderValidator
    {
        public CustomerId CustomerId { get; private set; }

        public OrderNumber OrderNumber { get; private set; }

        public async Task<ValidationResult> Validate(
            string rawOrderNumber,
            string rawCustomerId,
            IOrderHistory orderHistory,
            ICustomerHistory customerHistory)
        {
            var errors = new List<string>();
            this.ValidateArguments(rawOrderNumber, rawCustomerId, errors);
            await this.ValidateBusinessRules(orderHistory, customerHistory, errors);
            if (errors.Any())
            {
                return ValidationResult.Fails(errors);
            }

            return ValidationResult.Pass();
        }

        private void ValidateArguments(string rawOrderNumber, string rawCustomerId, List<string> errors)
        {
            this.OrderNumber = OrderNumber.Parse(rawOrderNumber);
            if (this.OrderNumber == null)
            {
                errors.Add("Order number in incorrect format");
            }

            this.CustomerId = CustomerId.Parse(rawCustomerId);
            if (this.CustomerId == null)
            {
                errors.Add("Customer id in incorrect format");
            }
        }

        private async Task ValidateBusinessRules(IOrderHistory orderHistory, ICustomerHistory customerHistory, List<string> errors)
        {
            if (this.OrderNumber != null)
            {
                if (await orderHistory.OrderExists(this.OrderNumber))
                {
                    errors.Add($"Order # {this.OrderNumber.Value} already exists");
                }
            }

            if (this.CustomerId != null)
            {
                if (await customerHistory.HasPlacedRecentOrder(this.CustomerId))
                {
                    errors.Add($"Customer # {this.CustomerId.Value} already has an order in progress");
                }
            }
        }
    }
}

namespace OrderingExample.Functions
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Application.Services;
    using Application.Validators;
    using Attributes;
    using DI;
    using Domain.Entities;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Persistence;
    using Serilog;

    public static class PlaceOrder
    {
        [FunctionName("PlaceAnOrder")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [Logger(Function = "PlaceAnOrder")] ILogger log,
            [Inject] IAggregateRepository repository,
            [Inject] IOrderHistory orderHistory,
            [Inject] ICustomerHistory customerHistory)
        {
            log.Information("PlaceOrder processing");
            var dict = req.GetQueryParameterDictionary();

            var validator = new PlaceOrderValidator();

            var result = await validator.Validate(
                GetValueOrDefault("OrderNumber", dict),
                GetValueOrDefault("CustomerId", dict),
                orderHistory,
                customerHistory);

            if (!result.HasPassed)
            {
                return new BadRequestObjectResult(result.Errors);
            }

            var order = new Order();
            order.Place(validator.CustomerId, validator.OrderNumber);
            await repository.Save(order);
            return new OkObjectResult("Thanks for placing an order");
        }

        private static string GetValueOrDefault(string key, IDictionary<string, string> dict)
        {
            if (dict.TryGetValue(key, out var value))
            {
                return value;
            }

            return string.Empty;
        }
    }
}

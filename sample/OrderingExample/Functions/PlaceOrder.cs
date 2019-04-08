namespace OrderingExample.Functions
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Application.Services;
    using Application.Validators;
    using Attributes;
    using DI;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using OrderingExample.Extensions;
    using Serilog;

    public static class PlaceOrder
    {
        [FunctionName("PlaceAnOrder")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [Logger(Function = "PlaceAnOrder")] ILogger log,
            [Inject] IOrderHistory orderHistory,
            [Inject] ICustomerHistory customerHistory,
            [Inject] IMediator mediator)
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

            var innerCmd = new Application.MediatrHandlers.PlaceOrder.Command(validator.OrderNumber, validator.CustomerId);
            await innerCmd.SendViaMessageQueue(mediator);
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

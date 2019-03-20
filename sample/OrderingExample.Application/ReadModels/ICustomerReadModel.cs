namespace OrderingExample.Application.ReadModels
{
    using System;
    using System.Threading.Tasks;

    public interface ICustomerReadModel
    {
        Task RecordOrderForCustomer(string customerId, DateTime cooldownPeriodExpires);

        Task RemoveOrderForCustomer(string customerId);
    }
}
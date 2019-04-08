namespace OrderingExample.Application.ReadModels
{
    using System;
    using System.Threading.Tasks;

    public interface IOrderReadModel
    {
        Task RecordNewOrder(string orderId, DateTime cooldownExpiry);
    }
}
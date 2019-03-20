namespace OrderingExample.Application.Services
{
    using System.Threading.Tasks;
    using Domain.ValueTypes;

    public interface IOrderHistory
    {
        Task<bool> OrderExists(OrderNumber orderNumber);
    }
}

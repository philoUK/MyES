namespace OrderingExample.Application.Services
{
    using System.Threading.Tasks;
    using Domain.ValueTypes;

    public interface ICustomerHistory
    {
        Task<bool> HasPlacedRecentOrder(CustomerId customerId);
    }
}

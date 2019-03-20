namespace Persistence
{
    using System.Threading.Tasks;
    using Core;

    public interface IAggregateRepository
    {
        Task Save(IAggregate aggregate);

        Task<T> Load<T>(string id)
            where T : IAggregate, new();
    }
}
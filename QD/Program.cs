using Persistence;
using Persistence.Azure;

namespace QD
{

    class Program
    {
        static void Main(string[] args)
        {
            var config = new TableStorageEventStoreConfiguration
            {
                ConnectionString = "UseDevelopmentStorage=true",
                EventTableName = "events",
                VersionTableName = "versions"
            };
            var store = new TableStorageEventStore(config);

            var registryConfig = new BlobStorageEventSubscriberRegistryConfig
            {
                ConnectionString = "UseDevelopmentStorage=true",
                ContainerName = "registry"
            };

            var registry = new BlobStorageEventSubsriberRegistry(registryConfig);

            var publishConfig = new QueuedEventPublisherConfig
            {
                ConnectionString = "UseDevelopmentStorage=true",
                QueueName = "events"
            };

            var publisher = new QueuedEventPublisher(publishConfig, registry);

            var repo = new AggregateRepository(store, publisher);

        }
    }
}

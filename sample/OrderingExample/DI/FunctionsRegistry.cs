namespace OrderingExample.DI
{
    using Application.Handlers;
    using Application.ReadModels;
    using Application.Services;
    using Azure;
    using Helpers;
    using MediatRExtensions;
    using Microsoft.Extensions.Options;
    using Persistence;
    using Persistence.Azure;
    using StructureMap;

    public class FunctionsRegistry : Registry
    {
        public FunctionsRegistry()
        {
            this.IncludeRegistry(new MediatrRegistry(typeof(ICustomerReadModel)));
            this.UseOptions<BlobStorageEventSubscriberRegistryConfig>("SubscriberRegistry");
            this.UseOptions<QueuedEventPublisherConfig>("EventPublisher");
            this.UseOptions<TableStorageEventStoreConfiguration>("EventStore");
            this.UseOptions<TableStorageCustomerReadModelConfiguration>("CustomerReadModel");
            this.UseOptions<TableStorageOrderReadModelConfiguration>("OrderReadModel");
            this.UseOptions<QueueCooldownWorkflowHandlerConfig>("Cooldown");
            this.ForConfig<BlobStorageEventSubscriberRegistryConfig>();
            this.ForConfig<QueuedEventPublisherConfig>();
            this.ForConfig<TableStorageEventStoreConfiguration>();
            this.ForConfig<TableStorageCustomerReadModelConfiguration>();
            this.ForConfig<TableStorageOrderReadModelConfiguration>();
            this.ForConfig<QueueCooldownWorkflowHandlerConfig>();
            this.For<IEventSubscriberRegistry>().DecorateAllWith<PubSubDecorator>().Singleton();
            this.For<IEventSubscriberRegistry>().Use<BlobStorageEventSubsriberRegistry>();
            this.For<IAggregateEventPublisher>().Use<QueuedEventPublisher>();
            this.For<IAggregateEventStore>().Use<TableStorageEventStore>();
            this.For<IAggregateRepository>().Use<AggregateRepository>();
            this.For<ICustomerReadModel>().Use<TableStorageCustomerReadModel>();
            this.For<IOrderReadModel>().Use<TableStorageOrderReadModel>();
            this.For<ICustomerHistory>().Use<TableStorageCustomerReadModel>();
            this.For<IOrderHistory>().Use<TableStorageOrderReadModel>();
            this.For<IHandlerFactory>().Use(ctx => new HandlerFactory(ctx));
            this.For<ICooldownWorkflowHandler>().Use<QueueCooldownWorkflowHandler>();
        }

        private void ForConfig<T>()
            where T : class, new()
        {
            this.For<T>().Use(ctx => ctx.GetInstance<IOptions<T>>().Value);
        }
    }
}

namespace OrderingExample.DI
{
    using Application.AggregateEventHandlers;
    using Application.ReadModels;
    using Application.Services;
    using Azure;
    using Config;
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
            this.IncludeRegistry(new MediatrRegistry(typeof(ICustomerReadModel), typeof(QueuedWrapper)));
            this.ForConfig<BlobStorageEventSubscriberRegistryConfig>("SubscriberRegistry");
            this.ForConfig<QueuedEventPublisherConfig>("EventPublisher");
            this.ForConfig<TableStorageEventStoreConfiguration>("EventStore");
            this.ForConfig<TableStorageCustomerReadModelConfiguration>("CustomerReadModel");
            this.ForConfig<TableStorageOrderReadModelConfiguration>("OrderReadModel");
            this.ForConfig<QueueCooldownWorkflowHandlerConfig>("Cooldown");
            this.ForConfig<QueuedWrapperConfig>("QueuedWrapper");
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

        private void ForConfig<T>(string configSection)
            where T : class, new()
        {
            this.For<IOptions<T>>().Use(() => new OptionsProvider<T>(configSection)).Singleton();
            this.For<T>().Use(ctx => ctx.GetInstance<IOptions<T>>().Value);
        }
    }
}

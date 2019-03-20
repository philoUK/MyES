namespace Persistence.Azure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Newtonsoft.Json;

    public class BlobStorageEventSubsriberRegistry : IEventSubscriberRegistry
    {
        private readonly BlobStorageEventSubscriberRegistryConfig config;
        private CloudBlobContainer container;
        private SubscriberRegistry registry;

        public BlobStorageEventSubsriberRegistry(BlobStorageEventSubscriberRegistryConfig config)
        {
            this.config = config;
        }

        public IEnumerable<Type> GetSubscribersForEvent(IAggregateEvent @event)
        {
            this.GetSubscriberRegistry();
            return this.registry.Subscriptions.Where(s => Type.GetType(s.EventType) == @event.GetType())
                .Select(s => Type.GetType(s.SubscriberType));
        }

        public void SubscribeToEvent(Type eventType, Type subscriberType)
        {
            this.GetSubscriberRegistry();
            if (!this.ContainSubscription(eventType, subscriberType))
            {
                this.registry.AddSubscription(eventType, subscriberType);
                this.SaveSubscriberRegistry();
            }
        }

        private bool ContainSubscription(Type eventType, Type subscriberType)
        {
            return this.registry.Subscriptions.Any(subscription =>
                subscription.EventType == eventType.AssemblyQualifiedName &&
                subscription.SubscriberType == subscriberType.AssemblyQualifiedName);
        }

        private void GetSubscriberRegistry()
        {
            if (this.registry == null)
            {
                this.InitialiseContainer();
                var blob = this.container.GetBlockBlobReference("registry.json");
                if (blob.ExistsAsync().Result)
                {
                    var json = blob.DownloadTextAsync().Result;
                    var results = JsonConvert.DeserializeObject(json, typeof(SubscriberRegistry));
                    this.registry = (SubscriberRegistry)results;
                }
                else
                {
                    this.registry = new SubscriberRegistry();
                }
            }
        }

        private void SaveSubscriberRegistry()
        {
            var blob = this.container.GetBlockBlobReference("registry.json");
            blob.UploadTextAsync(JsonConvert.SerializeObject(this.registry)).Wait();
        }

        private void InitialiseContainer()
        {
            if (this.container == null)
            {
                var account = CloudStorageAccount.Parse(this.config.ConnectionString);
                var client = account.CreateCloudBlobClient();
                this.container = client.GetContainerReference(this.config.ContainerName);
                this.container.CreateIfNotExistsAsync().Wait();
            }
        }

        private class Subscription
        {
            public string SubscriberType { get; set; }

            public string EventType { get; set; }
        }

        private class SubscriberRegistry
        {
            private readonly List<Subscription> subscriptions;

            public SubscriberRegistry()
            {
                this.subscriptions = new List<Subscription>();
            }

            public void AddSubscription(Type eventType, Type subscriberTtpe)
            {
                var newEntry = new Subscription
                {
                    EventType = eventType.AssemblyQualifiedName,
                    SubscriberType = subscriberTtpe.AssemblyQualifiedName
                };
                this.subscriptions.Add(newEntry);
            }

            public IEnumerable<Subscription> Subscriptions => this.subscriptions;
        }
    }
}

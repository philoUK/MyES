namespace Persistence.Azure
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Newtonsoft.Json;

    public class QueuedEventPublisher : IAggregateEventPublisher
    {
        // we also need the registry so we know who wants to receive the events
        private readonly QueuedEventPublisherConfig config;
        private readonly IEventSubscriberRegistry registry;

        private CloudQueue queue;

        public QueuedEventPublisher(QueuedEventPublisherConfig config, IEventSubscriberRegistry registry)
        {
            this.config = config;
            this.registry = registry;
        }

        public async Task Publish(IEnumerable<IAggregateEvent> events)
        {
            await this.CreateQueue();
            var tasks = new List<Task>();
            foreach (var @event in events)
            {
                this.PublishToSubscribers(@event, tasks);
            }

            await Task.WhenAll(tasks);
        }

        private async Task CreateQueue()
        {
            if (this.queue == null)
            {
                var storageAccount = CloudStorageAccount.Parse(this.config.ConnectionString);
                var client = storageAccount.CreateCloudQueueClient();
                this.queue = client.GetQueueReference(this.config.QueueName);
                await this.queue.CreateIfNotExistsAsync();
            }
        }

        private void PublishToSubscribers(IAggregateEvent @event, List<Task> tasks)
        {
            foreach (var type in this.registry.GetSubscribersForEvent(@event))
            {
                this.PublishToSubscriber(@event, tasks, type);
            }
        }

        private void PublishToSubscriber(IAggregateEvent @event, List<Task> tasks, Type type)
        {
            var notification = @event.ToEventPublishedNotification(type);
            var msg = new CloudQueueMessage(JsonConvert.SerializeObject(notification));
            tasks.Add(this.queue.AddMessageAsync(msg));
        }
    }
}

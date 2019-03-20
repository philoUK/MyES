namespace OrderingExample.Azure.Helpers
{
    using System.Threading.Tasks;
    using Core;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Newtonsoft.Json;

    internal class EventForwarder
    {
        private readonly string queueName;
        private readonly string connectionString;

        public EventForwarder(string connectionString, string queueName)
        {
            this.connectionString = connectionString;
            this.queueName = queueName;
        }

        public async Task Handle(IAggregateEvent @event)
        {
            var storageAccount = CloudStorageAccount.Parse(this.connectionString);
            var client = storageAccount.CreateCloudQueueClient();
            var cloudTable = client.GetQueueReference(this.queueName);
            await cloudTable.CreateIfNotExistsAsync();
            await cloudTable.AddMessageAsync(new CloudQueueMessage(JsonConvert.SerializeObject(@event)));
        }
    }
}

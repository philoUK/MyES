namespace MediatRExtensions
{
    using System.Threading;
    using System.Threading.Tasks;
    using Core.Utils;
    using MediatR;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Newtonsoft.Json;

    public static class QueuedWrapper
    {
        public class Command : IRequest
        {
            public Command()
            {
            }

            public Command(IRequest innerMessage)
            {
                this.WrappedType = innerMessage.GetType().ToLoadableString();
                this.WrappedJson = JsonConvert.SerializeObject(innerMessage);
            }

            public string WrappedType { get; set; }

            public string WrappedJson { get; set; }
        }

        public class Handler : AsyncRequestHandler<Command>
        {
            private readonly QueuedWrapperConfig config;

            public Handler(QueuedWrapperConfig config)
            {
                this.config = config;
            }

            protected override async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var storageAccount = CloudStorageAccount.Parse(this.config.ConnectionString);
                var client = storageAccount.CreateCloudQueueClient();
                var cloudTable = client.GetQueueReference(this.config.QueueName);
                await cloudTable.CreateIfNotExistsAsync();
                await cloudTable.AddMessageAsync(new CloudQueueMessage(JsonConvert.SerializeObject(request)));
            }
        }
    }
}

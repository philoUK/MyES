namespace Persistence.Azure
{
    public class QueuedEventPublisherConfig
    {
        public string ConnectionString { get; set; }

        public string QueueName { get; set; }
    }
}
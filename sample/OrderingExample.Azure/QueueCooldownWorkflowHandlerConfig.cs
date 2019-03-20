namespace OrderingExample.Azure
{
    public class QueueCooldownWorkflowHandlerConfig
    {
        public string ConnectionString { get; set; }

        public string QueueName { get; set; }
    }
}
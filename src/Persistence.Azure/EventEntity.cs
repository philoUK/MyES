namespace Persistence.Azure
{
    using Microsoft.WindowsAzure.Storage.Table;

    internal class EventEntity : TableEntity
    {
        public string EventName { get; set; }

        public string EventJson { get; set; }

        public string EventType { get; set; }
    }
}

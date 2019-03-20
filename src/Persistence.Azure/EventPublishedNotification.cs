namespace Persistence.Azure
{
    public class EventPublishedNotification
    {
        public string EventType { get; set; }

        public string EventJson { get; set; }

        public string SubscriberType { get; set; }
    }
}

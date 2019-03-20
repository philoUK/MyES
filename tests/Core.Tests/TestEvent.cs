namespace Core.Tests
{
    public class TestEvent : AggregateEvent
    {
        public TestEvent()
        {
        }

        public TestEvent(string value)
        {
            this.Value = value;
        }

        public string Value { get; set; }
    }
}

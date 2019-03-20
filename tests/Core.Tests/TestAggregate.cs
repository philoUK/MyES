namespace Core.Tests
{
    using System.Collections.Generic;

    public class TestAggregate : Aggregate, IDispatchAggregateEventsOf<TestEvent>,
        IDispatchAggregateEventsOf<AppendTestData>
    {
        private readonly List<string> testStrings;

        public TestAggregate()
        {
            this.testStrings = new List<string>();
        }

        public void ExecuteTestCommand(string commandData)
        {
            this.Apply(new TestEvent(commandData));
        }

        public void ExecuteAppendCommand(string testData)
        {
            this.Apply(new AppendTestData(testData));
        }

        public string TestData { get; private set; }

        public IEnumerable<string> TestStrings => this.testStrings;

        void IDispatchAggregateEventsOf<TestEvent>.Handle(TestEvent @event)
        {
            this.TestData = @event.Value;
        }

        void IDispatchAggregateEventsOf<AppendTestData>.Handle(AppendTestData @event)
        {
            this.testStrings.Add(@event.Data);
        }
    }
}
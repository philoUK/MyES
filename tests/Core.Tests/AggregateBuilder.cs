namespace Core.Tests
{
    public class AggregateBuilder
    {
        private readonly TestAggregate aggregate;

        public AggregateBuilder()
        {
            this.aggregate = new TestAggregate();
        }

        public AggregateBuilder WithCommand(string msg)
        {
            this.aggregate.ExecuteTestCommand(msg);
            return this;
        }

        public AggregateBuilder WithAppend(string data)
        {
            this.aggregate.ExecuteAppendCommand(data);
            return this;
        }

        public AggregateBuilder WithCommand()
        {
            return this.WithCommand("*");
        }

        public TestAggregate Build()
        {
            return this.aggregate;
        }
    }
}

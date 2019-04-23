namespace Core.Tests
{
    using Moq;
    using Persistence;

    internal class RepoBuilder
    {
        private readonly InMemoryEventStore store;
        private readonly Mock<IAggregateEventPublisher> eventPublisher;
        private TestAggregate aggregate;

        public RepoBuilder()
        {
            this.store = new InMemoryEventStore();
            this.eventPublisher = new Mock<IAggregateEventPublisher>();
        }

        public RepoBuilder WithNewAggregate()
        {
            this.aggregate = new AggregateBuilder().WithCommand().Build();
            return this;
        }

        public RepoBuilder WithExistingData()
        {
            this.store.GiveIncorrectLatestVersion();
            return this;
        }

        public RepoHarness Build()
        {
            return new RepoHarness(this.aggregate, this.store, this.eventPublisher);
        }

        public RepoBuilder WithAggregate(TestAggregate agg)
        {
            this.aggregate = agg;
            return this;
        }
    }
}

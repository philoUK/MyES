namespace Core.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Moq;
    using Persistence;
    using Xunit;

    internal class RepoHarness
    {
        private readonly TestAggregate aggregate;
        private readonly InMemoryEventStore eventStore;
        private readonly Mock<IAggregateEventPublisher> eventPublisher;
        private readonly AggregateRepository repository;

        public RepoHarness(TestAggregate aggregate, InMemoryEventStore eventStore, Mock<IAggregateEventPublisher> eventPublisher)
        {
            this.aggregate = aggregate;
            this.eventStore = eventStore;
            this.eventPublisher = eventPublisher;
            this.repository = new AggregateRepository(eventStore, eventPublisher.Object);
        }

        public Task Save()
        {
            return this.repository.Save(this.aggregate);
        }

        public void EnsureAnEventWasSavedToTheEventStore()
        {
            Assert.Equal(1, this.eventStore.EventCount);
        }

        public void EnsureThePublisherWasNotifiedOfTheEvent()
        {
            this.eventPublisher.Verify(ep => ep.Publish(It.IsAny<IEnumerable<IAggregateEvent>>()), Times.Once());
        }

        public void EnsureThePublisherWasNotNotifiedOfTheEvent()
        {
            this.eventPublisher.Verify(ep => ep.Publish(It.IsAny<IEnumerable<IAggregateEvent>>()), Times.Never());
        }

        public void EnsureNoEventsWereSavedToTheEventStore()
        {
            Assert.Equal(0, this.eventStore.EventCount);
        }

        public Task<TestAggregate> Load(string id)
        {
            return this.repository.Load<TestAggregate>(id);
        }
    }

    internal class RepoBuilder
    {
        private TestAggregate aggregate;
        private readonly InMemoryEventStore store;
        private readonly Mock<IAggregateEventPublisher> eventPublisher;

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

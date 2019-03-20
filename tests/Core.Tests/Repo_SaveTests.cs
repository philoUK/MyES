namespace Core.Tests
{
    using System.Threading.Tasks;
    using Persistence;
    using Xunit;

    public class Repo_SaveTests
    {
        // it saves each event to the event storage
        // it calls out to each interested party for each event

        // if there is an issue with concurrency, then we don't save the events
        // and we don't call the interested parties -- its effectively a "do-over"

        [Fact]
        public async Task SavesEachEventAndAllowsInterestedPartiesToDealWithEvents()
        {
            var harness = new RepoBuilder()
                .WithNewAggregate()
                .Build();

            await harness.Save();

            harness.EnsureAnEventWasSavedToTheEventStore();
            harness.EnsureThePublisherWasNotifiedOfTheEvent();
        }

        [Fact]
        public async Task IfSaveIsInterrupted_CorrectExceptionIsThrown()
        {
            var harness = new RepoBuilder()
                .WithNewAggregate()
                .WithExistingData()
                .Build();

            await Assert.ThrowsAsync<EventStoreConcurrencyException>(() => harness.Save());

            harness.EnsureNoEventsWereSavedToTheEventStore();
            harness.EnsureThePublisherWasNotNotifiedOfTheEvent();
        }
    }
}

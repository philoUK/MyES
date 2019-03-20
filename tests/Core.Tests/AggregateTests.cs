namespace Core.Tests
{
    using Xunit;

    public class AggregateTests
    {
        [Fact]
        public void InitialAggregateStateIsKnown()
        {
            var target = new TestAggregate();
            Assert.Equal(0, target.Version);
            Assert.Null(target.TestData);
            Assert.False(string.IsNullOrWhiteSpace(target.Id));
        }

        [Fact]
        public void ApplyIncrementsTheVersion()
        {
            var target = new AggregateBuilder().WithCommand().Build();
            Assert.Equal(1, target.Version);
        }

        [Fact]
        public void ApplyUpdatesTheState()
        {
            var state = "testdata";
            var target = new AggregateBuilder().WithCommand(state).Build();
            Assert.Equal("testdata", target.TestData);
        }

        [Fact]
        public void CanLoadNewAggregateToExactState()
        {
            var old = new AggregateBuilder().WithCommand().Build();
            var loaded = new TestAggregate();
            loaded.Load(old.PendingEvents());
            Assert.Equal(old, loaded);
        }
    }
}

namespace Core.Tests
{
    using System.Threading.Tasks;
    using Xunit;

    public class Repo_LoadTests
    {
        [Fact]
        public async Task AllEventsAreLoadedUpProperly()
        {
            var agg = new AggregateBuilder()
                .WithAppend("1")
                .WithAppend("2")
                .WithAppend("3")
                .WithCommand("*")
                .Build();
            var harness = new RepoBuilder()
                .WithAggregate(agg)
                .Build();
            await harness.Save();
            var agg2 = await harness.Load(agg.Id);
            Assert.Equal(agg, agg2);
        }
    }
}

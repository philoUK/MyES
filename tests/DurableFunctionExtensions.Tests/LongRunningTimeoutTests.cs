namespace DurableFunctionExtensions.Tests
{
    using System.Threading.Tasks;
    using Xunit;

    public class LongRunningTimeoutTests
    {
        private const string Utcnow = "2009-10-20 09:00";

        [Theory]
        [InlineData(Utcnow, "2009-10-20 09:00", 0)]
        [InlineData(Utcnow, "2009-10-20 08:59", 0)]
        [InlineData(Utcnow, "2009-10-27 09:00", 1)]
        [InlineData(Utcnow, "2009-10-25 23:58", 1)]
        [InlineData(Utcnow, "2009-11-03 09:00", 2)]
        [InlineData(Utcnow, "2009-11-03 09:01", 3)]
        public async Task DateLessThanOrEqualToMaxFiresOneTimer(string utcString, string askedForString, int expectedTimerQuantity)
        {
            var builder = new LongRunningTimeoutBuilder()
                .WithUtcDateOf(utcString)
                .WithAskedForDateOf(askedForString);

            await builder.Execute();
            builder.TimerCountShouldHaveBeen(expectedTimerQuantity);
        }
    }
}

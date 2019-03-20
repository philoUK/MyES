namespace DurableFunctionExtensions.Tests
{
    using System;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using Moq;

    internal class LongRunningTimeoutBuilder
    {
        private DateTime utcNow;
        private DateTime askedFor;
        private Mock<DurableOrchestrationContextBase> context;

        public LongRunningTimeoutBuilder WithUtcDateOf(string utcValue)
        {
            this.utcNow = DateTime.Parse(utcValue, new DateTimeFormatInfo());
            return this;
        }

        public LongRunningTimeoutBuilder WithAskedForDateOf(string askedForValue)
        {
            this.askedFor = DateTime.Parse(askedForValue, new DateTimeFormatInfo());
            return this;
        }

        public async Task Execute()
        {
            this.context = new Mock<DurableOrchestrationContextBase>();
            var tokenSource = new CancellationTokenSource();
            this.context.Setup(o => o.CurrentUtcDateTime).Returns(this.utcNow);
            await this.context.Object.CreateLongRunningTimer(this.askedFor, tokenSource.Token);
        }

        public void TimerCountShouldHaveBeen(int n)
        {
            this.context.Verify(ctx => ctx.CreateTimer(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Exactly(n));
        }
    }
}

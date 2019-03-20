namespace DurableFunctionExtensions
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;

    public static class TimeoutExtensions
    {
        public static async Task CreateLongRunningTimer(
            this DurableOrchestrationContextBase target,
            DateTime fireAt,
            CancellationToken token)
        {
            if (fireAt < target.CurrentUtcDateTime.AddDays(7))
            {
                await target.CreateTimer(fireAt, token);
            }
            else
            {
                var diff = fireAt - target.CurrentUtcDateTime;
                while (diff > TimeSpan.Zero)
                {
                    if (diff > TimeSpan.FromDays(7))
                    {
                        await target.CreateTimer(target.CurrentUtcDateTime.AddDays(7), token);
                        diff -= TimeSpan.FromDays(7);
                    }
                    else
                    {
                        await target.CreateTimer(target.CurrentUtcDateTime.Add(diff), token);
                        diff = TimeSpan.Zero;
                    }
                }
            }
        }
    }
}

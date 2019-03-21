namespace Monitoring.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using DurableTask.Core;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.DurableTask;
    using Microsoft.Extensions.Logging;

    public class MonitoredDurableTaskOrchestrationClient : DurableOrchestrationClientBase
    {
        private readonly DurableOrchestrationClientBase wrappedClient;
        private readonly ILogger logger;

        public MonitoredDurableTaskOrchestrationClient(DurableOrchestrationClientBase wrappedClient, ILogger logger)
        {
            this.wrappedClient = wrappedClient;
            this.logger = logger;
        }

        public override HttpResponseMessage CreateCheckStatusResponse(HttpRequestMessage request, string instanceId)
        {
            return this.wrappedClient.CreateCheckStatusResponse(request, instanceId);
        }

        public override HttpManagementPayload CreateHttpManagementPayload(string instanceId)
        {
            return this.wrappedClient.CreateHttpManagementPayload(instanceId);
        }

        public override Task<HttpResponseMessage> WaitForCompletionOrCreateCheckStatusResponseAsync(
            HttpRequestMessage request,
            string instanceId,
            TimeSpan timeout,
            TimeSpan retryInterval)
        {
            return this.wrappedClient.WaitForCompletionOrCreateCheckStatusResponseAsync(
                request,
                instanceId,
                timeout,
                retryInterval);
        }

        public override Task<string> StartNewAsync(string orchestratorFunctionName, string instanceId, object input)
        {
            this.logger.LogCritical("WRAPPER:  StartNewASYNC");
            return this.wrappedClient.StartNewAsync(orchestratorFunctionName, instanceId, input);
        }

        public override Task RaiseEventAsync(string instanceId, string eventName, object eventData)
        {
            return this.wrappedClient.RaiseEventAsync(instanceId, eventName, eventData);
        }

        public override Task RaiseEventAsync(
            string taskHubName,
            string instanceId,
            string eventName,
            object eventData,
            string connectionName = null)
        {
            return this.wrappedClient.RaiseEventAsync(taskHubName, instanceId, eventName, eventData, connectionName);
        }

        public override Task TerminateAsync(string instanceId, string reason)
        {
            return this.wrappedClient.TerminateAsync(instanceId, reason);
        }

        public override Task RewindAsync(string instanceId, string reason)
        {
            return this.wrappedClient.RewindAsync(instanceId, reason);
        }

        public override Task<DurableOrchestrationStatus> GetStatusAsync(
            string instanceId,
            bool showHistory,
            bool showHistoryOutput,
            bool showInput = true)
        {
            return this.wrappedClient.GetStatusAsync(instanceId, showHistory, showHistoryOutput, showInput);
        }

        public override Task<IList<DurableOrchestrationStatus>> GetStatusAsync(
            CancellationToken cancellationToken = new CancellationToken())
        {
            return this.wrappedClient.GetStatusAsync(cancellationToken);
        }

        public override Task<IList<DurableOrchestrationStatus>> GetStatusAsync(
            DateTime createdTimeFrom,
            DateTime? createdTimeTo,
            IEnumerable<OrchestrationRuntimeStatus> runtimeStatus,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return this.wrappedClient.GetStatusAsync(createdTimeFrom, createdTimeTo, runtimeStatus, cancellationToken);
        }

        public override Task<PurgeHistoryResult> PurgeInstanceHistoryAsync(string instanceId)
        {
            return this.wrappedClient.PurgeInstanceHistoryAsync(instanceId);
        }

        public override Task<PurgeHistoryResult> PurgeInstanceHistoryAsync(
            DateTime createdTimeFrom,
            DateTime? createdTimeTo,
            IEnumerable<OrchestrationStatus> runtimeStatus)
        {
            return this.wrappedClient.PurgeInstanceHistoryAsync(createdTimeFrom, createdTimeTo, runtimeStatus);
        }

        public override Task<OrchestrationStatusQueryResult> GetStatusAsync(
            DateTime createdTimeFrom,
            DateTime? createdTimeTo,
            IEnumerable<OrchestrationRuntimeStatus> runtimeStatus,
            int pageSize,
            string continuationToken,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return this.wrappedClient.GetStatusAsync(
                createdTimeFrom,
                createdTimeTo,
                runtimeStatus,
                pageSize,
                continuationToken,
                cancellationToken);
        }

        public override string TaskHubName => this.wrappedClient.TaskHubName;
    }
}

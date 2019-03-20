namespace Persistence.Azure
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;

    public class TableStorageEventStore : IAggregateEventStore
    {
        private readonly TableStorageEventStoreConfiguration configuration;
        private CloudTable eventStoreTable;
        private CloudTable eventVersionTable;

        public TableStorageEventStore(TableStorageEventStoreConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<int> GetLatestVersionOf(string aggregateId)
        {
            this.GetOrCreateVersionTable();
            var op = TableOperation.Retrieve<LatestVersionEntity>(aggregateId, string.Empty);
            var result = await this.eventVersionTable.ExecuteAsync(op);
            if (result.Result == null)
            {
                return 0;
            }

            return ((LatestVersionEntity)result.Result).Version;
        }

        private void GetOrCreateVersionTable()
        {
            this.eventVersionTable =
                this.GetOrCreateTable(this.eventVersionTable, this.configuration.VersionTableName);
        }

        public async Task Store(IEnumerable<IAggregateEvent> events)
        {
            this.GetOrCreateEventStoreTable();
            var orderedEvents = events.OrderBy(e => e.Version).ToList();
            foreach (var batch in orderedEvents.ToBatch(100))
            {
                await this.StoreEntity(batch);
            }

            await this.SaveLatestVersion(orderedEvents);
        }

        public async Task<IEnumerable<IAggregateEvent>> GetEventsForId(string id)
        {
            this.GetOrCreateEventStoreTable();
            var query = new TableQuery<EventEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, id));
            var results = new List<IAggregateEvent>();
            TableContinuationToken token = null;
            do
            {
                var queryResult = await this.eventStoreTable.ExecuteQuerySegmentedAsync(query, token);
                results.AddRange(queryResult.Results.Select(entity => entity.ToEvent()));
                token = queryResult.ContinuationToken;
            }
            while (token != null);

            return results;
        }

        private async Task StoreEntity(IEnumerable<IAggregateEvent> events)
        {
            var op = new TableBatchOperation();
            foreach (var @event in events.Select(ae => ae.ToEventEntity()))
            {
                op.Add(TableOperation.Insert(@event));
            }

            await this.eventStoreTable.ExecuteBatchAsync(op);
        }

        private void GetOrCreateEventStoreTable()
        {
            this.eventStoreTable =
                this.GetOrCreateTable(this.eventStoreTable, this.configuration.EventTableName);
        }

        private async Task SaveLatestVersion(IEnumerable<IAggregateEvent> events)
        {
            var last = events.OrderBy(e => e.Version).LastOrDefault();
            if (last != null)
            {
                var entity = last.ToLatestVersionEntity();
                var op = TableOperation.InsertOrReplace(entity);
                await this.eventVersionTable.ExecuteAsync(op);
            }
        }

        private CloudTable GetOrCreateTable(CloudTable table, string tableName)
        {
            if (table != null)
            {
                return table;
            }

            var storageAccount = CloudStorageAccount.Parse(this.configuration.ConnectionString);
            var client = storageAccount.CreateCloudTableClient();
            var result = client.GetTableReference(tableName);
            result.CreateIfNotExistsAsync().Wait();
            return result;
        }
    }
}

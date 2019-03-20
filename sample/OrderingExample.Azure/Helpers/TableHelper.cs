namespace OrderingExample.Azure.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;

    internal class TableHelper
    {
        private readonly string connectionString;
        private readonly string tableName;
        private CloudTable table;

        public TableHelper(string connectionString, string tableName)
        {
            this.connectionString = connectionString;
            this.tableName = tableName;
        }

        public async Task InsertOrReplace(ITableEntity entity)
        {
            await this.ExecuteAsync(TableOperation.InsertOrReplace(entity));
        }

        public async Task Insert(ITableEntity entity)
        {
            await this.ExecuteAsync(TableOperation.Insert(entity));
        }

        public async Task Delete(string partitionKey, string rowId)
        {
            var entity = await this.GetItemByPartitionKeyAndRowKey<DynamicTableEntity>(partitionKey, rowId);
            await this.table.ExecuteAsync(TableOperation.Delete(entity));
        }

        public async Task<List<T>> GetItemsByPartitionKey<T>(string value)
            where T : ITableEntity, new()
        {
            this.GetOrCreateTable();
            var query = new TableQuery<T>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, value));
            var results = new List<T>();
            TableContinuationToken token = null;
            do
            {
                var result = await this.table.ExecuteQuerySegmentedAsync(query, token);
                results.AddRange(result.Results);
                token = result.ContinuationToken;
            }
            while (token != null);
            return results;
        }

        public async Task<T> GetItemByPartitionKeyAndRowKey<T>(string partitionKey, string rowKey)
            where T : ITableEntity, new()
        {
            this.GetOrCreateTable();
            var query = new TableQuery<T>()
                .Where(
                    TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey),
                        TableOperators.And,
                        TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, rowKey)));
            var results = new List<T>();
            TableContinuationToken token = null;
            do
            {
                var result = await this.table.ExecuteQuerySegmentedAsync(query, token);
                results.AddRange(result.Results);
                token = result.ContinuationToken;
            }
            while (token != null);

            return results.FirstOrDefault();
        }

        private async Task ExecuteAsync(TableOperation op)
        {
            this.GetOrCreateTable();
            await this.table.ExecuteAsync(op);
        }

        private void GetOrCreateTable()
        {
            if (this.table == null)
            {
                var storageAccount = CloudStorageAccount.Parse(this.connectionString);
                var client = storageAccount.CreateCloudTableClient();
                this.table = client.GetTableReference(this.tableName);
                this.table.CreateIfNotExistsAsync().Wait();
            }
        }
    }
}

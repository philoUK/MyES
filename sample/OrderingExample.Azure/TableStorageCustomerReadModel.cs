namespace OrderingExample.Azure
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Application.ReadModels;
    using Application.Services;
    using Domain.ValueTypes;
    using Helpers;

    public class TableStorageCustomerReadModel : ICustomerReadModel, ICustomerHistory
    {
        private readonly TableHelper tableHelper;

        public TableStorageCustomerReadModel(TableStorageCustomerReadModelConfiguration config)
        {
            this.tableHelper = new TableHelper(config.ConnectionString, config.TableName);
        }

        public async Task RecordOrderForCustomer(string customerId, DateTime cooldownPeriodExpires)
        {
            var entity = new CustomerReadModelEntity
            {
                PartitionKey = customerId,
                RowKey = string.Empty,
                CooldownPeriodExpires = cooldownPeriodExpires
            };
            await this.tableHelper.InsertOrReplace(entity);
        }

        public async Task RemoveOrderForCustomer(string customerId)
        {
            await this.tableHelper.Delete(customerId, string.Empty);
        }

        public async Task<bool> HasPlacedRecentOrder(CustomerId customerId)
        {
            var items = await this.tableHelper.GetItemsByPartitionKey<CustomerReadModelEntity>(customerId.Value);
            return items.Any(e => e.CooldownPeriodExpires >= DateTime.UtcNow);
        }
    }
}

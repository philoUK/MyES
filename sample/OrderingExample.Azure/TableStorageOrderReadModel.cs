namespace OrderingExample.Azure
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Application.ReadModels;
    using Application.Services;
    using Domain.ValueTypes;
    using Helpers;

    public class TableStorageOrderReadModel : IOrderReadModel, IOrderHistory
    {
        private readonly TableHelper tableHelper;

        public TableStorageOrderReadModel(TableStorageOrderReadModelConfiguration config)
        {
            this.tableHelper = new TableHelper(config.ConnectionString, config.TableName);
        }

        public async Task RecordNewOrder(string orderId, DateTime cooldownExpiry)
        {
            var entity = new OrderReadModelEntity
            {
                PartitionKey = orderId,
                RowKey = string.Empty,
                CooldownPeriodExpires = cooldownExpiry
            };
            await this.tableHelper.Insert(entity);
        }

        public async Task RemoveOrder(string orderId)
        {
            await this.tableHelper.Delete(orderId, string.Empty);
        }

        public async Task<bool> OrderExists(OrderNumber orderNumber)
        {
            var results = await this.tableHelper.GetItemsByPartitionKey<OrderReadModelEntity>(orderNumber.Value);
            return results.Any();
        }
    }
}

namespace OrderingExample.Azure
{
    using System;
    using Microsoft.WindowsAzure.Storage.Table;

    internal class CustomerReadModelEntity : TableEntity
    {
        public DateTime CooldownPeriodExpires { get; set; }
    }
}

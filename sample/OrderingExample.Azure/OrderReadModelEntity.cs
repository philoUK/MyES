namespace OrderingExample.Azure
{
    using System;
    using Microsoft.WindowsAzure.Storage.Table;

    internal class OrderReadModelEntity : TableEntity
    {
        public DateTime CooldownPeriodExpires { get; set; }
    }
}

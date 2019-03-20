namespace Persistence.Azure
{
    public class TableStorageEventStoreConfiguration
    {
        public string ConnectionString { get; set; }

        public string EventTableName { get; set; }

        public string VersionTableName { get; set; }
    }
}

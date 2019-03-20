namespace Persistence.Azure
{
    public class BlobStorageEventSubscriberRegistryConfig
    {
        public string ConnectionString { get; set; }

        public string ContainerName { get; set; }
    }
}
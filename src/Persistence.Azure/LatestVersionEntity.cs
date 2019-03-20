namespace Persistence.Azure
{
    using Microsoft.WindowsAzure.Storage.Table;

    internal class LatestVersionEntity : TableEntity
    {
        public int Version { get; set; }
    }
}

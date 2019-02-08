namespace MidnightLizard.Impressions.Infrastructure.Configuration
{
    public abstract class ElasticSearchConfig
    {
        public string ClientUrl { get; set; }

        public string EventStoreIndexName { get; set; }
        public int EventStoreShards { get; set; } = 2;
        public int EventStoreReplicas { get; set; } = 1;

        public string SnapshotIndexName { get; set; }
        public int SnapshotShards { get; set; } = 2;
        public int SnapshotReplicas { get; set; } = 0;

    }
}
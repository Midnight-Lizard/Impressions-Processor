namespace MidnightLizard.Impressions.Infrastructure.Configuration
{
    public class ElasticSearchConfig
    {
        public string ELASTIC_SEARCH_CLIENT_URL { get; set; }

        public string ELASTIC_SEARCH_EVENT_STORE_IMPRESSIONS_INDEX_NAME { get; set; }
        public int ELASTIC_SEARCH_EVENT_STORE_SHARDS { get; set; } = 2;
        public int ELASTIC_SEARCH_EVENT_STORE_REPLICAS { get; set; } = 1;

        public string ELASTIC_SEARCH_SNAPSHOT_IMPRESSIONS_INDEX_NAME { get; set; }
        public int ELASTIC_SEARCH_SNAPSHOT_SHARDS { get; set; } = 2;
        public int ELASTIC_SEARCH_SNAPSHOT_REPLICAS { get; set; } = 0;

    }
}
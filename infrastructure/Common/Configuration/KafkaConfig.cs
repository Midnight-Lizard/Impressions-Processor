using System.Collections.Generic;

namespace MidnightLizard.Impressions.Infrastructure.Configuration
{
    public class KafkaConfig
    {
        public Dictionary<string, object> KAFKA_EVENTS_CONSUMER_CONFIG { get; set; }
        public Dictionary<string, object> KAFKA_REQUESTS_CONSUMER_CONFIG { get; set; }
        public Dictionary<string, object> KAFKA_EVENTS_PRODUCER_CONFIG { get; set; }
        public Dictionary<string, object> KAFKA_REQUESTS_PRODUCER_CONFIG { get; set; }

        public IEnumerable<string> EVENT_TOPICS { get; set; }
        public IEnumerable<string> REQUEST_TOPICS { get; set; }
    }
}
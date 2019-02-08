using System.Collections.Generic;

namespace MidnightLizard.Impressions.Infrastructure.Configuration
{
    public class LikesKafkaConfig
    {
        public string LIKES_INTERNAL_EVENTS_TOPIC { get; set; }
        public string LIKES_INTEGRATION_EVENTS_TOPIC { get; set; }
        public string LIKES_FAILED_EVENTS_TOPIC { get; set; }
    }
}
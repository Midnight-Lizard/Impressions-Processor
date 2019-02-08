using System.Collections.Generic;

namespace MidnightLizard.Impressions.Infrastructure.Configuration
{
    public class FavoritesKafkaConfig
    {
        public string FAVORITES_INTERNAL_EVENTS_TOPIC { get; set; }
        public string FAVORITES_INTEGRATION_EVENTS_TOPIC { get; set; }
        public string FAVORITES_FAILED_EVENTS_TOPIC { get; set; }
    }
}
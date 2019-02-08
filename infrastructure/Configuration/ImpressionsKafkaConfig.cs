using System.Collections.Generic;

namespace MidnightLizard.Impressions.Infrastructure.Configuration
{
    public class ImpressionsKafkaConfig
    {
        public string IMPRESSIONS_INTERNAL_EVENTS_TOPIC { get; set; }
        public string IMPRESSIONS_INTEGRATION_EVENTS_TOPIC { get; set; }
        public string IMPRESSIONS_FAILED_EVENTS_TOPIC { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Impressions.Processor.Configuration
{
    public class AggregatesConfig
    {
        public bool AGGREGATE_CACHE_ENABLED { get; set; }
        public int AGGREGATE_CACHE_SLIDING_EXPIRATION_SECONDS { get; set; }
        public int AGGREGATE_CACHE_ABSOLUTE_EXPIRATION_SECONDS { get; set; }
        public int AGGREGATE_MAX_EVENTS_COUNT { get; set; }
    }
}

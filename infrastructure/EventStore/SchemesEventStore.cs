using MidnightLizard.SImpressionsDomain.PublicSchemeAggregate;
using MidnightLizard.Impressions.Infrastructure.Configuration;
using MidnightLizard.Impressions.Infrastructure.Serialization.Common;

namespace MidnightLizard.Impressions.Infrastructure.EventStore
{
    public class ImpressionsEventStore : DomainEventStore<PublicSchemeId>
    {
        protected override string IndexName => this.config.ELASTIC_SEARCH_EVENT_STORE_IMPRESSIONS_INDEX_NAME;

        public ImpressionsEventStore(ElasticSearchConfig config, IMessageSerializer messageSerializer) :
            base(config, messageSerializer)
        {
        }
    }
}

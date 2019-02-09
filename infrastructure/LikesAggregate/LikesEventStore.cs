using MidnightLizard.Impressions.Domain.LikesAggregate;
using MidnightLizard.Impressions.Infrastructure.Configuration;
using MidnightLizard.Impressions.Infrastructure.Serialization.Common;

namespace MidnightLizard.Impressions.Infrastructure.EventStore
{
    public class LikesEventStore : DomainEventStore<LikesId>
    {
        public LikesEventStore(LikesElasticSearchConfig config, IMessageSerializer messageSerializer) :
            base(config, messageSerializer)
        {
        }
    }
}

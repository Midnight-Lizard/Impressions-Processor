using MidnightLizard.Impressions.Domain.FavoritesAggregate;
using MidnightLizard.Impressions.Domain.ImpressionsAggregate;
using MidnightLizard.Impressions.Infrastructure.Configuration;
using MidnightLizard.Impressions.Infrastructure.Serialization.Common;

namespace MidnightLizard.Impressions.Infrastructure.EventStore
{
    public class FavoritesEventStore : DomainEventStore<FavoritesId>
    {
        public FavoritesEventStore(FavoritesElasticSearchConfig config, IMessageSerializer messageSerializer) :
            base(config, messageSerializer)
        {
        }
    }
}

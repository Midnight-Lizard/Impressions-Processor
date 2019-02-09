using MidnightLizard.Impressions.Domain.ImpressionsAggregate;
using MidnightLizard.Impressions.Domain.ImpressionsAggregate.Events;

namespace MidnightLizard.Impressions.Domain.FavoritesAggregate.Events
{
    public class RemovedFromFavoritesEvent : ImpressionRemovedEvent<FavoritesId>
    {
        protected RemovedFromFavoritesEvent() : base() { }

        public RemovedFromFavoritesEvent(FavoritesId aggregateId, ImpressionsObjectType objectType)
            : base(aggregateId, objectType)
        { }
    }
}

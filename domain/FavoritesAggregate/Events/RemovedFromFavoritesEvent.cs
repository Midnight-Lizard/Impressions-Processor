using MidnightLizard.Impressions.Domain.ImpressionsAggregate;
using MidnightLizard.Impressions.Domain.ImpressionsAggregate.Events;

namespace MidnightLizard.Impressions.Domain.FavoritesAggregate.Events
{
    public class RemovedFromFavoritesEvent : ImpressionRemovedEvent
    {
        protected RemovedFromFavoritesEvent() : base() { }

        public RemovedFromFavoritesEvent(ImpressionsObjectId aggregateId, ImpressionsObjectType objectType)
            : base(aggregateId, objectType)
        { }
    }
}

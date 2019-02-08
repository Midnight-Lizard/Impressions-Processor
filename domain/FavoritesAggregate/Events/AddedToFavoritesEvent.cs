using MidnightLizard.Impressions.Domain.ImpressionsAggregate;
using MidnightLizard.Impressions.Domain.ImpressionsAggregate.Events;

namespace MidnightLizard.Impressions.Domain.FavoritesAggregate.Events
{
    public class AddedToFavoritesEvent : ImpressionAddedEvent
    {
        protected AddedToFavoritesEvent() : base() { }

        public AddedToFavoritesEvent(ImpressionsObjectId aggregateId, ImpressionsObjectType objectType)
            : base(aggregateId, objectType)
        { }
    }
}

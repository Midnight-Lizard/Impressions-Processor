using MidnightLizard.Impressions.Domain.ImpressionsAggregate;
using MidnightLizard.Impressions.Domain.ImpressionsAggregate.Events;

namespace MidnightLizard.Impressions.Domain.FavoritesAggregate.Events
{
    public class AddedToFavoritesEvent : ImpressionAddedEvent<FavoritesId>
    {
        protected AddedToFavoritesEvent() : base() { }

        public AddedToFavoritesEvent(FavoritesId aggregateId, ImpressionsObjectType objectType)
            : base(aggregateId, objectType)
        { }
    }
}

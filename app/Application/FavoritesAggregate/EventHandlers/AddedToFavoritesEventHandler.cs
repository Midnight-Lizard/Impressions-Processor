using MidnightLizard.Commons.Domain.Interfaces;
using MidnightLizard.Impressions.Domain.FavoritesAggregate;
using MidnightLizard.Impressions.Domain.FavoritesAggregate.Events;
using MidnightLizard.Impressions.Processor.Application.DomainEventHandlers;

namespace MidnightLizard.Impressions.Processor.Application.FavoritesAggregate.EventHandlers
{
    public class AddedToFavoritesEventHandler
        : EventSourcedDomainEventHandler<AddedToFavoritesEvent, FavoritesId>
    {
        public AddedToFavoritesEventHandler(IDomainEventStore<FavoritesId> eventStore)
            : base(eventStore)
        {
        }
    }
}

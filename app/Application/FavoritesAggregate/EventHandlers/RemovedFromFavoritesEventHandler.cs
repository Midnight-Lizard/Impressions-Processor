using MidnightLizard.Commons.Domain.Interfaces;
using MidnightLizard.Impressions.Domain.FavoritesAggregate;
using MidnightLizard.Impressions.Domain.FavoritesAggregate.Events;
using MidnightLizard.Impressions.Processor.Application.DomainEventHandlers;

namespace MidnightLizard.Impressions.Processor.Application.FavoritesAggregate.EventHandlers
{
    public class RemovedFromFavoritesEventHandler
        : EventSourcedDomainEventHandler<RemovedFromFavoritesEvent, FavoritesId>
    {
        public RemovedFromFavoritesEventHandler(IDomainEventStore<FavoritesId> eventStore)
            : base(eventStore)
        {
        }
    }
}

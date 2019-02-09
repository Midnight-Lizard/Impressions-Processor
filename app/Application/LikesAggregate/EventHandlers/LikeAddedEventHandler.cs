using MidnightLizard.Commons.Domain.Interfaces;
using MidnightLizard.Impressions.Domain.LikesAggregate;
using MidnightLizard.Impressions.Domain.LikesAggregate.Events;
using MidnightLizard.Impressions.Processor.Application.DomainEventHandlers;

namespace MidnightLizard.Impressions.Processor.Application.LikesAggregate.EventHandlers
{
    public class LikeAddedEventHandler : EventSourcedDomainEventHandler<LikeAddedEvent, LikesId>
    {
        public LikeAddedEventHandler(IDomainEventStore<LikesId> eventStore)
            : base(eventStore)
        {
        }
    }
}

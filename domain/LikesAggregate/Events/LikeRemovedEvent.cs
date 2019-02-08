using MidnightLizard.Impressions.Domain.ImpressionsAggregate;
using MidnightLizard.Impressions.Domain.ImpressionsAggregate.Events;

namespace MidnightLizard.Impressions.Domain.LikesAggregate.Events
{
    public class LikeRemovedEvent : ImpressionRemovedEvent
    {
        protected LikeRemovedEvent() : base() { }

        public LikeRemovedEvent(ImpressionsObjectId aggregateId, ImpressionsObjectType objectType)
            : base(aggregateId, objectType)
        { }
    }
}

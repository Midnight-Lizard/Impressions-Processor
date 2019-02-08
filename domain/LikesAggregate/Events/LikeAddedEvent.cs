using MidnightLizard.Impressions.Domain.ImpressionsAggregate;
using MidnightLizard.Impressions.Domain.ImpressionsAggregate.Events;

namespace MidnightLizard.Impressions.Domain.LikesAggregate.Events
{
    public class LikeAddedEvent : ImpressionAddedEvent
    {
        protected LikeAddedEvent() : base() { }

        public LikeAddedEvent(ImpressionsObjectId aggregateId, ImpressionsObjectType objectType)
            : base(aggregateId, objectType)
        { }
    }
}

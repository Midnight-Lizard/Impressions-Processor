using MidnightLizard.Impressions.Domain.ImpressionsAggregate;
using MidnightLizard.Impressions.Domain.ImpressionsAggregate.Events;

namespace MidnightLizard.Impressions.Domain.LikesAggregate.Events
{
    public class LikeAddedEvent : ImpressionAddedEvent<LikesId>
    {
        protected LikeAddedEvent() : base() { }

        public LikeAddedEvent(LikesId aggregateId, ImpressionsObjectType objectType)
            : base(aggregateId, objectType)
        { }
    }
}

using MidnightLizard.Impressions.Domain.ImpressionsAggregate;
using MidnightLizard.Impressions.Domain.ImpressionsAggregate.Events;

namespace MidnightLizard.Impressions.Domain.LikesAggregate.Events
{
    public class LikeRemovedEvent : ImpressionRemovedEvent<LikesId>
    {
        protected LikeRemovedEvent() : base() { }

        public LikeRemovedEvent(LikesId aggregateId, ImpressionsObjectType objectType)
            : base(aggregateId, objectType)
        { }
    }
}

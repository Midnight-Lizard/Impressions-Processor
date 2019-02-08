using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Impressions.Domain.ImpressionsAggregate;
using MidnightLizard.Impressions.Domain.ImpressionsAggregate.Events;
using System.Collections.Generic;

namespace MidnightLizard.Impressions.Domain.LikesAggregate.Events
{
    public class LikesChangedEvent : ImpressionsChangedEvent
    {
        public int TotalLikes => this.impressionists.Count;
        public IReadOnlyCollection<UserId> LikedBy
        {
            get => this.impressionists;
            protected set => this.impressionists = value;
        }

        protected LikesChangedEvent() : base() { }

        public LikesChangedEvent(
            ImpressionsObjectId aggregateId,
            ImpressionsObjectType objectType,
            IReadOnlyCollection<UserId> likedBy) : base(aggregateId, objectType, likedBy)
        { }
    }
}

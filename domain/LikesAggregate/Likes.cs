using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Impressions.Domain.ImpressionsAggregate;
using MidnightLizard.Impressions.Domain.ImpressionsAggregate.Events;
using MidnightLizard.Impressions.Domain.LikesAggregate.Events;
using System.Collections.Generic;

namespace MidnightLizard.Impressions.Domain.LikesAggregate
{
    public class Likes : ImpressionsAggregate.Impressions
    {
        public IReadOnlyCollection<UserId> LikedBy
        {
            get => this._Impressionists;
            private set => this._Impressionists = new HashSet<UserId>(value);
        }

        protected override ImpressionAddedEvent CreateImpressionAddedEvent(ImpressionsObjectType objectType)
        {
            return new LikeAddedEvent(this.Id, objectType);
        }

        protected override ImpressionRemovedEvent CreateImpressionRemovedEvent(ImpressionsObjectType objectType)
        {
            return new LikeRemovedEvent(this.Id, objectType);
        }

        protected override ImpressionsChangedEvent CreateImpressionsChangedEvent()
        {
            return new LikesChangedEvent(this.Id, this.ObjectType, this.LikedBy);
        }
    }
}

using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Impressions.Domain.FavoritesAggregate.Events;
using MidnightLizard.Impressions.Domain.ImpressionsAggregate;
using MidnightLizard.Impressions.Domain.ImpressionsAggregate.Events;
using System.Collections.Generic;

namespace MidnightLizard.Impressions.Domain.FavoritesAggregate
{
    public class Favorites : ImpressionsAggregate.Impressions
    {
        public IReadOnlyCollection<UserId> FavoritedBy
        {
            get => this._Impressionists;
            private set => this._Impressionists = new HashSet<UserId>(value);
        }

        protected override ImpressionAddedEvent CreateImpressionAddedEvent(ImpressionsObjectType objectType)
        {
            return new AddedToFavoritesEvent(this.Id, objectType);
        }

        protected override ImpressionRemovedEvent CreateImpressionRemovedEvent(ImpressionsObjectType objectType)
        {
            return new RemovedFromFavoritesEvent(this.Id, objectType);
        }

        protected override ImpressionsChangedEvent CreateImpressionsChangedEvent()
        {
            return new FavoritesChangedEvent(this.Id, this.ObjectType, this.FavoritedBy);
        }
    }
}

﻿using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Impressions.Domain.FavoritesAggregate.Events;
using MidnightLizard.Impressions.Domain.ImpressionsAggregate;
using MidnightLizard.Impressions.Domain.ImpressionsAggregate.Events;
using System.Collections.Generic;

namespace MidnightLizard.Impressions.Domain.FavoritesAggregate
{
    public class Favorites : ImpressionsAggregate.Impressions<FavoritesId>
    {
        protected Favorites() : base() { }
        public Favorites(FavoritesId aggregateId) : base(aggregateId) { }

        public IReadOnlyCollection<UserId> FavoritedBy
        {
            get => this._Impressionists;
            private set => this._Impressionists = new HashSet<UserId>(value);
        }

        protected override ImpressionAddedEvent<FavoritesId> CreateImpressionAddedEvent(ImpressionsObjectType objectType)
        {
            return new AddedToFavoritesEvent(this.Id, objectType);
        }

        protected override ImpressionRemovedEvent<FavoritesId> CreateImpressionRemovedEvent(ImpressionsObjectType objectType)
        {
            return new RemovedFromFavoritesEvent(this.Id, objectType);
        }

        protected override ImpressionsChangedEvent<FavoritesId> CreateImpressionsChangedEvent()
        {
            return new FavoritesChangedEvent(this.Id, this.ObjectType, this.FavoritedBy);
        }
    }
}

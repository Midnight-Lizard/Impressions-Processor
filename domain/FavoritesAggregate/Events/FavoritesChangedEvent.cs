using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Impressions.Domain.ImpressionsAggregate;
using MidnightLizard.Impressions.Domain.ImpressionsAggregate.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Impressions.Domain.FavoritesAggregate.Events
{
    public class FavoritesChangedEvent : ImpressionsChangedEvent<FavoritesId>
    {
        public int TotalFavoriters => this.impressionists.Count;
        public IReadOnlyCollection<UserId> FavoritedBy
        {
            get => this.impressionists;
            protected set => this.impressionists = value;
        }

        protected FavoritesChangedEvent() : base() { }

        public FavoritesChangedEvent(
            FavoritesId aggregateId,
            ImpressionsObjectType objectType,
            IReadOnlyCollection<UserId> favoritedBy) : base(aggregateId, objectType, favoritedBy)
        { }
    }
}

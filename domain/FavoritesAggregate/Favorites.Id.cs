using MidnightLizard.Impressions.Domain.ImpressionsAggregate;

namespace MidnightLizard.Impressions.Domain.FavoritesAggregate
{
    public class FavoritesId : ImpressionsObjectId
    {
        protected FavoritesId() : base() { }
        public FavoritesId(string id) : base(id) { }
    }
}

using MidnightLizard.Impressions.Domain.ImpressionsAggregate;

namespace MidnightLizard.Impressions.Domain.LikesAggregate
{
    public class LikesId : ImpressionsObjectId
    {
        protected LikesId() : base() { }
        public LikesId(string id) : base(id) { }
    }
}

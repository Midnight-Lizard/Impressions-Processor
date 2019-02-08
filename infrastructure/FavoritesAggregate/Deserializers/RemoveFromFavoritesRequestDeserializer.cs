using MidnightLizard.Impressions.Domain.FavoritesAggregate.Requests;
using MidnightLizard.Impressions.Domain.ImpressionsAggregate;
using MidnightLizard.Impressions.Infrastructure.Serialization.Common;

namespace MidnightLizard.Impressions.Infrastructure.Serialization.Deserializers
{
    [Message(Version = ">=1")]
    public class RemoveFromFavoritesRequestDeserializer_v1 : AbstractMessageDeserializer<RemoveFromFavoritesRequest, ImpressionsObjectId>
    {
        public override void StartAdvancingToTheLatestVersion(RemoveFromFavoritesRequest message)
        {
            base.AdvanceToTheLatestVersion(message);
        }
    }
}

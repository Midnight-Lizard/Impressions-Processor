using MidnightLizard.Impressions.Domain.FavoritesAggregate;
using MidnightLizard.Impressions.Domain.FavoritesAggregate.Requests;
using MidnightLizard.Impressions.Infrastructure.Serialization.Common;

namespace MidnightLizard.Impressions.Infrastructure.Serialization.Deserializers
{
    [Message(Version = ">=1")]
    public class AddToFavoritesRequestDeserializer_v1 : AbstractMessageDeserializer<AddToFavoritesRequest, FavoritesId>
    {
        public override void StartAdvancingToTheLatestVersion(AddToFavoritesRequest message)
        {
            base.AdvanceToTheLatestVersion(message);
        }
    }
}

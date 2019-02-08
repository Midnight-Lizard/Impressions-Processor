using MidnightLizard.Impressions.Domain.FavoritesAggregate.Requests;
using MidnightLizard.Impressions.Domain.ImpressionsAggregate;
using MidnightLizard.Impressions.Infrastructure.Serialization.Common;

namespace MidnightLizard.Impressions.Infrastructure.Serialization.Deserializers
{
    [Message(Version = ">=1")]
    public class AddToFavoritesRequestDeserializer_v1 : AbstractMessageDeserializer<AddToFavoritesRequest, ImpressionsObjectId>
    {
        public override void StartAdvancingToTheLatestVersion(AddToFavoritesRequest message)
        {
            base.AdvanceToTheLatestVersion(message);
        }
    }
}

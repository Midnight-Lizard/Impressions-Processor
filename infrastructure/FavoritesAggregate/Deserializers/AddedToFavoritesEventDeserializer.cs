using MidnightLizard.Impressions.Domain.FavoritesAggregate;
using MidnightLizard.Impressions.Domain.FavoritesAggregate.Events;
using MidnightLizard.Impressions.Infrastructure.Serialization.Common;

namespace MidnightLizard.Impressions.Infrastructure.Serialization.Deserializers
{
    [Message(Version = ">=1")]
    public class AddedToFavoritesEventDeserializer_v1
        : AbstractMessageDeserializer<AddedToFavoritesEvent, FavoritesId>
    {
        public override void StartAdvancingToTheLatestVersion(AddedToFavoritesEvent message)
        {
            base.AdvanceToTheLatestVersion(message);
        }
    }
}

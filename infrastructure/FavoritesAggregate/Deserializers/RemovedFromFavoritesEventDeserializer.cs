using MidnightLizard.Impressions.Domain.FavoritesAggregate;
using MidnightLizard.Impressions.Domain.FavoritesAggregate.Events;
using MidnightLizard.Impressions.Infrastructure.Serialization.Common;

namespace MidnightLizard.Impressions.Infrastructure.Serialization.Deserializers
{
    [Message(Version = ">=1")]
    public class RemovedFromFavoritesEventDeserializer_v1
        : AbstractMessageDeserializer<RemovedFromFavoritesEvent, FavoritesId>
    {
        public override void StartAdvancingToTheLatestVersion(RemovedFromFavoritesEvent message)
        {
            base.AdvanceToTheLatestVersion(message);
        }
    }
}

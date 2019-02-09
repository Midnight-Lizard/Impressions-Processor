using MidnightLizard.Impressions.Domain.LikesAggregate;
using MidnightLizard.Impressions.Domain.LikesAggregate.Events;
using MidnightLizard.Impressions.Infrastructure.Serialization.Common;

namespace MidnightLizard.Impressions.Infrastructure.LikesAggregate.Deserializers
{
    [Message(Version = ">=1")]
    public class LikeRemovedEventDeserializer_v1
        : AbstractMessageDeserializer<LikeRemovedEvent, LikesId>
    {
        public override void StartAdvancingToTheLatestVersion(LikeRemovedEvent message)
        {
            base.AdvanceToTheLatestVersion(message);
        }
    }
}

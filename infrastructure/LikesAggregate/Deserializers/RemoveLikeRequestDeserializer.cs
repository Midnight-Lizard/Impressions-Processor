using MidnightLizard.Impressions.Domain.LikesAggregate;
using MidnightLizard.Impressions.Domain.LikesAggregate.Requests;
using MidnightLizard.Impressions.Infrastructure.Serialization.Common;

namespace MidnightLizard.Impressions.Infrastructure.Serialization.Deserializers
{
    [Message(Version = ">=1")]
    public class RemoveLikeRequestDeserializer_v1
        : AbstractMessageDeserializer<RemoveLikeRequest, LikesId>
    {
        public override void StartAdvancingToTheLatestVersion(RemoveLikeRequest message)
        {
            base.AdvanceToTheLatestVersion(message);
        }
    }
}

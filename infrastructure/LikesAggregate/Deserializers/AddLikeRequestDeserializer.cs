using MidnightLizard.Impressions.Domain.LikesAggregate;
using MidnightLizard.Impressions.Domain.LikesAggregate.Requests;
using MidnightLizard.Impressions.Infrastructure.Serialization.Common;

namespace MidnightLizard.Impressions.Infrastructure.Serialization.Deserializers
{
    [Message(Version = ">=1")]
    public class AddLikeRequestDeserializer_v1
        : AbstractMessageDeserializer<AddLikeRequest, LikesId>
    {
        public override void StartAdvancingToTheLatestVersion(AddLikeRequest message)
        {
            base.AdvanceToTheLatestVersion(message);
        }
    }
}

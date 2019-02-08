﻿using MidnightLizard.Impressions.Domain.ImpressionsAggregate;
using MidnightLizard.Impressions.Domain.LikesAggregate.Events;
using MidnightLizard.Impressions.Infrastructure.Serialization.Common;

namespace MidnightLizard.Impressions.Infrastructure.LikesAggregate.Deserializers
{
    [Message(Version = ">=1")]
    public class LikeAddedEventDeserializer_v1 : AbstractMessageDeserializer<LikeAddedEvent, ImpressionsObjectId>
    {
        public override void StartAdvancingToTheLatestVersion(LikeAddedEvent message)
        {
            base.AdvanceToTheLatestVersion(message);
        }
    }
}

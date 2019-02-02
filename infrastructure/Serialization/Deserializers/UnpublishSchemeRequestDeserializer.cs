using MidnightLizard.Commons.Domain.Messaging;
using MidnightLizard.Impressions.Domain.PublicSchemeAggregate;
using MidnightLizard.Impressions.Domain.PublicSchemeAggregate.Requests;
using MidnightLizard.Impressions.Infrastructure.Serialization.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Impressions.Infrastructure.Serialization.Deserializers
{
    [Message(Version = "*")]
    public class UnpublishSchemeRequestDeserializer : AbstractMessageDeserializer<UnpublishSchemeRequest, PublicSchemeId>
    {
        public override void StartAdvancingToTheLatestVersion(UnpublishSchemeRequest message)
        {
        }
    }
}

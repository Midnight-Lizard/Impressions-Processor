using MediatR;
using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Commons.Domain.Messaging;
using MidnightLizard.Commons.Domain.Results;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Impressions.Infrastructure.Serialization.Common
{
    public interface IMessageDeserializer
    {

    }

    public interface IMessageDeserializer<out TMessage> : IMessageDeserializer where TMessage : BaseMessage
    {
        ITransportMessage<TMessage> DeserializeMessagePayload(
            string payload, JsonSerializerSettings serializerSettings,
            Guid correlationId, UserId userId, DateTime requestTimestamp, DateTime eventTimestamp);
    }
}

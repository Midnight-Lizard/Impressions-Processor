using Elasticsearch.Net;
using MidnightLizard.Commons.Domain.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MidnightLizard.Impressions.Infrastructure.Serialization.Common
{
    public class DomainEventSerializer : IElasticsearchSerializer
    {
        private readonly IMessageSerializer messageSerializer;

        public DomainEventSerializer(IMessageSerializer messageSerializer)
        {
            this.messageSerializer = messageSerializer;
        }

        public object Deserialize(Type type, Stream stream)
        {
            if (typeof(ITransportMessage<BaseMessage>).IsAssignableFrom(type))
            {
                using (var reader = new StreamReader(stream))
                {
                    var result = messageSerializer.Deserialize(reader.ReadToEnd());
                    if (!result.HasError)
                    {
                        return result.Message;
                    }
                }
            }
            return null;
        }

        public T Deserialize<T>(Stream stream)
        {
            return (T)this.Deserialize(typeof(T), stream);
        }

        public async Task<object> DeserializeAsync(Type type, Stream stream, CancellationToken cancellationToken = default)
        {
            if (typeof(ITransportMessage<BaseMessage>).IsAssignableFrom(type))
            {
                using (var reader = new StreamReader(stream))
                {
                    var result = messageSerializer.Deserialize(await reader.ReadToEndAsync());
                    if (!result.HasError)
                    {
                        return result.Message;
                    }
                }
            }
            return null;
        }

        public async Task<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default)
        {
            return (T)await this.DeserializeAsync(typeof(T), stream, cancellationToken);
        }

        public void Serialize<T>(T data, Stream stream, SerializationFormatting formatting = SerializationFormatting.Indented)
        {
            string json = SerializeToString(data);
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(json);
            }
        }

        public async Task SerializeAsync<T>(T data, Stream stream, SerializationFormatting formatting = SerializationFormatting.Indented, CancellationToken cancellationToken = default)
        {
            string json = SerializeToString(data);
            using (var writer = new StreamWriter(stream))
            {
                await writer.WriteAsync(json);
            }
        }

        private string SerializeToString<T>(T data)
        {
            string json = "";
            switch (data)
            {
                case ITransportMessage<BaseMessage> message:
                    json = this.messageSerializer.SerializeMessage(message);
                    break;

                case var obj:
                    json = this.messageSerializer.SerializeObject(obj);
                    break;
            }

            return json;
        }
    }
}

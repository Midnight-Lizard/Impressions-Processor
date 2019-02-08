using Elasticsearch.Net;
using MidnightLizard.Impressions.Infrastructure.Serialization.Common.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MidnightLizard.Impressions.Infrastructure.Serialization.Common
{
    public class AggregateSerializer : IElasticsearchSerializer
    {
        private readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            ContractResolver = OrderedContractResolver.Default,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            Converters = new JsonConverter[] {
                    new DomainEntityIdConverter(),
                    new SingleValueTypeConverter(),
                    new StringEnumConverter(new CamelCaseNamingStrategy())
                }
        };

        public object Deserialize(Type type, Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                return DeserializeFromString(reader.ReadToEnd(), type);
            }
        }

        public T Deserialize<T>(Stream stream)
        {
            return (T)this.Deserialize(typeof(T), stream);
        }

        public async Task<object> DeserializeAsync(Type type, Stream stream, CancellationToken cancellationToken = default)
        {
            using (var reader = new StreamReader(stream))
            {
                return DeserializeFromString(await reader.ReadToEndAsync(), type);
            }
        }

        public async Task<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default)
        {
            return (T)await this.DeserializeAsync(typeof(T), stream, cancellationToken);
        }

        public void Serialize<T>(T data, Stream stream, SerializationFormatting formatting = SerializationFormatting.Indented)
        {
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(SerializeToString(data));
            }
        }

        public async Task SerializeAsync<T>(T data, Stream stream, SerializationFormatting formatting = SerializationFormatting.Indented, CancellationToken cancellationToken = default)
        {
            using (var writer = new StreamWriter(stream))
            {
                await writer.WriteAsync(SerializeToString(data));
            }
        }

        private string SerializeToString<T>(T data)
        {
            return JsonConvert.SerializeObject(data, this.serializerSettings);
        }

        private object DeserializeFromString(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type, this.serializerSettings);
        }
    }
}
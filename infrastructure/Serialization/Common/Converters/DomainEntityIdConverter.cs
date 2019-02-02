using MidnightLizard.Commons.Domain.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MidnightLizard.Impressions.Infrastructure.Serialization.Common.Converters
{
    public class DomainEntityIdConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(DomainEntityId).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var domainEntityId = Activator.CreateInstance(objectType, true);
            var valProp = objectType.GetProperty(nameof(DomainEntityId<int>.Value));
            var valueConverter = TypeDescriptor.GetConverter(valProp.PropertyType);
            valProp.SetValue(domainEntityId,
                valueConverter.ConvertFromString(reader.Value as string),
                BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.NonPublic,
                null, null, null);
            return domainEntityId;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is DomainEntityId<Guid> domainEntityIdGuid)
            {
                writer.WriteValue(domainEntityIdGuid.Value.ToString());
            }
            else if (value is DomainEntityId<string> domainEntityIdString)
            {
                writer.WriteValue(domainEntityIdString.Value ?? "");
            }
        }
    }
}

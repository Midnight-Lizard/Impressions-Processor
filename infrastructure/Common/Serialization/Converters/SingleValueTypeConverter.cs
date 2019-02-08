using MidnightLizard.Commons.Domain.Model;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Reflection;

namespace MidnightLizard.Impressions.Infrastructure.Serialization.Common.Converters
{
    public class SingleValueTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(SingleValueType).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var domainEntityId = Activator.CreateInstance(objectType, true);
            var valProp = objectType.GetProperty(nameof(SingleValueType<int>.Value));
            var valueConverter = TypeDescriptor.GetConverter(valProp.PropertyType);
            valProp.SetValue(domainEntityId,
                valueConverter.ConvertFromString(reader.Value as string),
                BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.NonPublic,
                null, null, null);
            return domainEntityId;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is SingleValueType<Guid> guidVal)
            {
                writer.WriteValue(guidVal.Value.ToString());
            }
            else if (value is SingleValueType<int> intVal)
            {
                writer.WriteValue(intVal.Value.ToString());
            }
            else if (value is SingleValueType<string> stringVal)
            {
                writer.WriteValue(stringVal.Value ?? "");
            }
        }
    }
}

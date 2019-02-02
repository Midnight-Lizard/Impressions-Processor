using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MidnightLizard.Impressions.Infrastructure.Serialization.Common
{

    public class MessageContractResolver : DefaultContractResolver
    {
        public static readonly MessageContractResolver Default = new MessageContractResolver();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            property.Writable = property.Writable ||
                member is PropertyInfo propInfo && propInfo.GetSetMethod(true) != null;

            return property;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization);
            if (properties != null)
            {
                return properties.OrderBy(p => typeLevels.GetOrAdd(p.DeclaringType, GetTypeLevel)).ToList();
            }
            return properties;
        }

        private static int GetTypeLevel(Type type)
        {
            var level = 0;
            var baseType = type.BaseType;
            while (baseType != null && baseType.Namespace != null &&
                baseType.Namespace.StartsWith(nameof(MidnightLizard)))
            {
                baseType = baseType.BaseType;
                level++;
            }
            return level;
        }

        private static readonly ConcurrentDictionary<Type, int> typeLevels = new ConcurrentDictionary<Type, int>();
    }
}
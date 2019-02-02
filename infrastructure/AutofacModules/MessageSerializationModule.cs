using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core;
using Autofac.Features.Variance;
using System.Threading.Tasks;
using System.Reflection;
using MidnightLizard.Impressions.Infrastructure.Queue;
using MediatR;
using MidnightLizard.Commons.Domain.Results;
using MidnightLizard.Commons.Domain.Messaging;
using MidnightLizard.Impressions.Infrastructure.Serialization.Common;

namespace MidnightLizard.Impressions.Infrastructure.AutofacModules
{
    public class MessageSerializationModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MessageSerializer>()
                .As<IMessageSerializer>()
                .SingleInstance();

            builder.RegisterAssemblyTypes(typeof(MessageSerializationModule).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IMessageDeserializer<>))
                .As<IMessageDeserializer>()
                .WithMetadata(t =>
                {
                    var msgAttr = t.GetCustomAttribute<MessageAttribute>();
                    return new Dictionary<string, object>
                    {
                        [nameof(IMessageMetadata.Type)] = msgAttr.Type ?? t.GetInterfaces().First().GetGenericArguments()[0].Name,
                        [nameof(IMessageMetadata.VersionRange)] = msgAttr.VersionRange
                    };
                });
            //.WithMetadataFrom<MessageAttribute>();
            //.WithMetadata<MessageAttribute>(t=>t.For<)
            //.Keyed<IMessageDeserializer>(t =>
            //{
            //    var msgAttr = t.GetCustomAttribute<MessageAttribute>();
            //    var msgType = msgAttr.Type ?? t.GetInterfaces().First().GetGenericArguments()[0].Name;
            //    return $"{msgType}{msgAttr.Version}";
            //});
        }
    }
}

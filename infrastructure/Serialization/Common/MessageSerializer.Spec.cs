using Autofac;
using FluentAssertions;
using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Impressions.Domain.PublicSchemeAggregate;
using MidnightLizard.Impressions.Domain.PublicSchemeAggregate.Events;
using MidnightLizard.Impressions.Infrastructure.AutofacModules;
using MidnightLizard.Impressions.Infrastructure.Versioning;
using MidnightLizard.Testing.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using Event = MidnightLizard.Impressions.Domain.PublicSchemeAggregate.Events.SchemePublishedEvent;
using TransEvent = MidnightLizard.Commons.Domain.Messaging.TransportMessage<MidnightLizard.Impressions.Domain.PublicSchemeAggregate.Events.SchemePublishedEvent, MidnightLizard.Impressions.Domain.PublicSchemeAggregate.PublicSchemeId>;

namespace MidnightLizard.Impressions.Infrastructure.Serialization.Common
{
    public class MessageSerializerSpec
    {
        private readonly string description = "test description";
        private readonly IMessageSerializer messageSerializer;
        private readonly TransEvent testTransEvent;

        public MessageSerializerSpec()
        {
            this.testTransEvent = new TransEvent(
                new SchemePublishedEvent(
                    new PublicSchemeId(Guid.NewGuid()),
                    ColorSchemeSpec.CorrectColorScheme,
                    this.description),
                Guid.NewGuid(), new UserId("test-user-id"), DateTime.UtcNow, DateTime.UtcNow.AddHours(1));

            var builder = new ContainerBuilder();
            builder.RegisterModule<MessageSerializationModule>();
            builder.RegisterInstance(SchemaVersion.Latest);
            var container = builder.Build();
            this.messageSerializer = container.Resolve<IMessageSerializer>();
        }

        public class SerializeSpec : MessageSerializerSpec
        {
            [It(nameof(MessageSerializer.SerializeMessage))]
            public void Should_correctly_Serialize_event()
            {
                var json = this.messageSerializer.SerializeMessage(this.testTransEvent);
                var obj = JObject.Parse(json);

                obj[nameof(TransEvent.CorrelationId)].ToObject<Guid>().Should().Be(this.testTransEvent.CorrelationId);
                obj[nameof(Type)].Value<string>().Should().Be(nameof(SchemePublishedEvent));
                obj[nameof(Version)].Value<string>().Should().Be(SchemaVersion.Latest.ToString());
                obj[nameof(UserId)].Value<string>().Should().Be(this.testTransEvent.UserId.Value);
                obj[nameof(TransEvent.RequestTimestamp)].Value<DateTime>().Should().Be(this.testTransEvent.RequestTimestamp);
                obj[nameof(TransEvent.EventTimestamp)].Value<DateTime?>().Should().Be(this.testTransEvent.EventTimestamp);

                var payload = obj[nameof(TransEvent.Payload)];

                payload[nameof(Event.Id)].ToObject<Guid>().Should().Be(this.testTransEvent.Payload.Id);
                payload[nameof(Event.AggregateId)].ToObject<Guid>().Should().Be(this.testTransEvent.Payload.AggregateId.Value);
                payload[nameof(Event.Generation)].Value<int>().Should().Be(this.testTransEvent.Payload.Generation);
                payload[nameof(Event.ColorScheme)].ToObject<ColorScheme>().Should().Be(this.testTransEvent.Payload.ColorScheme);
            }
        }

        public class DeserializeSpec : MessageSerializerSpec
        {
            [It(nameof(MessageSerializer.Deserialize))]
            public void Should_return_an_error_if_message_version_is_not_supported()
            {
                var json = $@"
                {{
                    ""CorrelationId"": ""{Guid.NewGuid()}"",
                    ""Type"": ""{nameof(SchemePublishedEvent)}"",
                    ""Version"": ""0.0.0"",
                    ""RequestTimestamp"": ""{DateTime.UtcNow}"",
                    ""Payload"": {{}}
                }}";

                var result = this.messageSerializer.Deserialize(json, DateTime.UtcNow);

                result.HasError.Should().BeTrue();
                result.ErrorMessage.Should().Contain(nameof(SchemePublishedEvent));
                result.ErrorMessage.Should().Contain("0.0.0");
            }

            [It(nameof(MessageSerializer.Deserialize))]
            public void Should_return_an_error_if_message_has_incorrect_json_format()
            {
                var json = $@"
                {{
                    ""CorrelationId"": ""not a GUID"",
                    ""Type"": ""{nameof(SchemePublishedEvent)}"",
                    ""Version"": ""0.0.0"",
                    ""RequestTimestamp"": ""not a DateTime"",
                    ""Payload"": {{}}
                }}";

                var result = this.messageSerializer.Deserialize(json, DateTime.UtcNow);

                result.HasError.Should().BeTrue();
                result.Exception.Should().NotBeNull();
            }

            [It(nameof(MessageSerializer.Deserialize))]
            public void Should_correctly_Deserialize_event()
            {
                var te = this.testTransEvent;
                te.Payload.Generation = 3;
                var json = $@"
                {{
                    ""CorrelationId"": ""{te.CorrelationId}"",
                    ""Type"": ""{nameof(SchemePublishedEvent)}"",
                    ""Version"": ""{SchemaVersion.Latest}"",
                    ""RequestTimestamp"": {JsonConvert.SerializeObject(te.RequestTimestamp)},
                    ""EventTimestamp"": {JsonConvert.SerializeObject(te.EventTimestamp)},
                    ""UserId"": ""{te.UserId}"",
                    ""Payload"": {{
                        ""Id"": ""{te.Payload.Id}"",
                        ""AggregateId"": ""{te.Payload.AggregateId}"",
                        ""Generation"": {te.Payload.Generation},
                        ""ColorScheme"": {te.Payload.ColorScheme}
                    }}
                }}";

                var result = this.messageSerializer.Deserialize(json, DateTime.UtcNow);

                result.HasError.Should().BeFalse();

                var msg = result.Message;
                msg.DeserializerType.Should().Be<Deserializers.SchemePublishedEventDeserializer_v10_1>();
                msg.CorrelationId.Should().Be(te.CorrelationId);
                msg.RequestTimestamp.Should().Be(te.RequestTimestamp);
                msg.EventTimestamp.Should().Be(te.EventTimestamp);
                msg.UserId.Should().Be(te.UserId);

                msg.Payload.Should().BeOfType<SchemePublishedEvent>();

                var e = msg.Payload as SchemePublishedEvent;
                e.Id.Should().Be(te.Payload.Id);
                e.AggregateId.Should().Be(te.Payload.AggregateId);
                e.Generation.Should().Be(te.Payload.Generation);
                e.ColorScheme.Should().Be(te.Payload.ColorScheme);
            }
        }
    }
}

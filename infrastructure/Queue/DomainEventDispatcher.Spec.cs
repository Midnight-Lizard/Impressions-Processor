using Confluent.Kafka;
using FluentAssertions;
using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Impressions.Domain.PublicSchemeAggregate;
using MidnightLizard.Impressions.Domain.PublicSchemeAggregate.Events;
using MidnightLizard.Impressions.Infrastructure.Configuration;
using MidnightLizard.Impressions.Infrastructure.Serialization.Common;
using MidnightLizard.Testing.Utilities;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using TransEvent = MidnightLizard.Commons.Domain.Messaging.TransportMessage<MidnightLizard.Commons.Domain.Messaging.DomainEvent<MidnightLizard.Impressions.Domain.PublicSchemeAggregate.PublicSchemeId>, MidnightLizard.Impressions.Domain.PublicSchemeAggregate.PublicSchemeId>;

namespace MidnightLizard.Impressions.Infrastructure.Queue
{
    public class DomainEventDispatcherSpec : DomainEventDispatcher<PublicSchemeId>
    {
        private readonly string description = "test description";
        private readonly string testMessageJson = "{Type:\"Test\"}";
        private readonly UserId testUserId = new UserId("test-user-id");
        private int GetEventTopicName_CallCount = 0;
        private readonly TransEvent testTransEvent;

        public DomainEventDispatcherSpec() : base(
            new KafkaConfig
            {
                KAFKA_EVENTS_PRODUCER_CONFIG = new Dictionary<string, object>
                {
                    ["bootstrap.servers"] = "test:123"
                }
            },
            Substitute.For<IMessageSerializer>())
        {
            this.testTransEvent = new TransEvent(
               new SchemePublishedEvent(new PublicSchemeId(Guid.NewGuid()), ColorSchemeSpec.CorrectColorScheme, this.description),
               Guid.NewGuid(), this.testUserId, DateTime.UtcNow, DateTime.UtcNow);
            this.producer = Substitute.For<ISerializingProducer<string, string>>();
            this.producer.ProduceAsync(
                this.GetEventTopicName(),
                this.testTransEvent.Payload.AggregateId.ToString(),
                Arg.Any<string>())
                .Returns(new Message<string, string>("", 0, 0, "", "", new Timestamp(), new Error(ErrorCode.NoError)));
            this.messageSerializer.SerializeMessage(this.testTransEvent).Returns(this.testMessageJson);
        }

        //private readonly IDomainEventDispatcher<PublicSchemeId> eventDispatcher;

        //public DomainEventDispatcherSpec()
        //{
        //var builder = new ContainerBuilder();
        //builder.RegisterModule<DomainInfrastructureModule>();
        //builder.RegisterInstance<KafkaConfig>(new KafkaConfig());
        //var container = builder.Build();
        //eventDispatcher = container.Resolve<IDomainEventDispatcher<PublicSchemeId>>();
        //}

        protected override string GetEventTopicName()
        {
            this.GetEventTopicName_CallCount++;
            return "Test";
        }

        public class DispatchEventSpec : DomainEventDispatcherSpec
        {
            [It(nameof(DispatchEvent))]
            public async Task Should_serialize_TransportEvent()
            {
                var result = await this.DispatchEvent(this.testTransEvent);

                this.messageSerializer.Received(1).SerializeMessage(this.testTransEvent);
            }

            [It(nameof(DispatchEvent))]
            public async Task Should_call_GetEventTopicName()
            {
                this.GetEventTopicName_CallCount = 0;

                var result = await this.DispatchEvent(this.testTransEvent);

                this.GetEventTopicName_CallCount.Should().Be(1);
            }

            [It(nameof(DispatchEvent))]
            public async Task Should_Produce_KafkaMessage()
            {
                var result = await this.DispatchEvent(this.testTransEvent);

                await this.producer.Received(1).ProduceAsync(
                    this.GetEventTopicName(),
                    this.testTransEvent.Payload.AggregateId.ToString(),
                    this.testMessageJson);
            }

            [It(nameof(DispatchEvent))]
            public async Task Should_return_Error_when_Exception_thrown()
            {
                this.messageSerializer.SerializeMessage(this.testTransEvent)
                    .Returns(x => throw new Exception());

                var result = await this.DispatchEvent(this.testTransEvent);

                result.HasError.Should().BeTrue();
                result.Exception.Should().NotBeNull();
            }

            [It(nameof(DispatchEvent))]
            public async Task Should_return_Error_when_Producer_does()
            {
                this.producer.ProduceAsync(
                    this.GetEventTopicName(),
                    this.testTransEvent.Payload.AggregateId.ToString(),
                    this.testMessageJson).Returns(new Message<string, string>("", 0, 0, "", "", new Timestamp(),
                        new Error(ErrorCode.Unknown)));

                var result = await this.DispatchEvent(this.testTransEvent);

                result.HasError.Should().BeTrue();
            }
        }
    }
}

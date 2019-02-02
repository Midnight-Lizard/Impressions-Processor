using Autofac;
using Elasticsearch.Net;
using FluentAssertions;
using JsonDiffPatchDotNet;
using MidnightLizard.Commons.Domain.Messaging;
using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Impressions.Domain.PublicSchemeAggregate;
using MidnightLizard.Impressions.Domain.PublicSchemeAggregate.Events;
using MidnightLizard.Impressions.Infrastructure.AutofacModules;
using MidnightLizard.Impressions.Infrastructure.Configuration;
using MidnightLizard.Impressions.Infrastructure.Serialization.Common;
using MidnightLizard.Impressions.Infrastructure.Versioning;
using MidnightLizard.Testing.Utilities;
using Nest;
using Newtonsoft.Json.Linq;
using NSubstitute;
using System;
using System.Text;
using System.Threading.Tasks;
using ITransEvent = MidnightLizard.Commons.Domain.Messaging.ITransportMessage<MidnightLizard.Commons.Domain.Messaging.BaseMessage>;
using TransEvent = MidnightLizard.Commons.Domain.Messaging.TransportMessage<MidnightLizard.Commons.Domain.Messaging.DomainEvent<MidnightLizard.Impressions.Domain.PublicSchemeAggregate.PublicSchemeId>, MidnightLizard.Impressions.Domain.PublicSchemeAggregate.PublicSchemeId>;

namespace MidnightLizard.Impressions.Infrastructure.EventStore
{
    public abstract class DomainEventStoreSpec : DomainEventStore<PublicSchemeId>
    {
        protected override string IndexName => "test";
        protected abstract void OnRequestCompleted(IApiCallDetails x);
        private readonly UserId testUserId = new UserId("test-user-id");
        private IMessageSerializer realMessageSerializer;
        private readonly TransEvent testTransEvent;
        private readonly string description = "test description";

        public DomainEventStoreSpec() : base(
            Substitute.For<ElasticSearchConfig>(),
            Substitute.For<IMessageSerializer>())
        {
            this.testTransEvent = new TransEvent(
               new SchemePublishedEvent(new PublicSchemeId(Guid.NewGuid()),
                   ColorSchemeSpec.CorrectColorScheme, this.description),
               Guid.NewGuid(), this.testUserId, DateTime.UtcNow, DateTime.UtcNow);
        }

        protected override ElasticClient CreateElasticClient()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<MessageSerializationModule>();
            builder.RegisterInstance(SchemaVersion.Latest);
            var container = builder.Build();
            this.realMessageSerializer = container.Resolve<IMessageSerializer>();

            return new ElasticClient(this.InitDefaultMapping(new ConnectionSettings(
                new SingleNodeConnectionPool(new Uri("http://test.com")), new InMemoryConnection(),
                (builtin, settings) => new DomainEventSerializer(this.realMessageSerializer))
                    .EnableDebugMode(this.OnRequestCompleted)
            ));
        }

        public class CreateIndexSpec : DomainEventStoreSpec
        {
            private readonly JObject createIndexCommandSnapshot = JObject.Parse("{\"settings\":{\"index.number_of_replicas\":1,\"index.number_of_shards\":2},\"mappings\":{\"event\":{\"_routing\":{\"required\":true},\"properties\":{\"Type\":{\"type\":\"keyword\"},\"Version\":{\"type\":\"keyword\"},\"CorrelationId\":{\"type\":\"keyword\"},\"RequestTimestamp\":{\"type\":\"date\"},\"EventTimestamp\":{\"type\":\"date\"},\"Payload\":{\"type\":\"object\",\"properties\":{\"Generation\":{\"type\":\"integer\"},\"AggregateId\":{\"type\":\"keyword\"},\"Id\":{\"type\":\"keyword\"}}}}}}}");
            private JObject createIndexCommand;

            protected override void OnRequestCompleted(IApiCallDetails x)
            {
                if (x.RequestBodyInBytes != null && x.RequestBodyInBytes.Length > 1)
                {
                    this.createIndexCommand = JObject.Parse(Encoding.UTF8.GetString(x.RequestBodyInBytes));
                }
            }

            [It(nameof(CreateIndex))]
            public void Should_issue_correct_CreateIndex_command()
            {
                this.CreateIndex();
                if (!JToken.DeepEquals(this.createIndexCommand, this.createIndexCommandSnapshot))
                {
                    new JsonDiffPatch()
                        .Diff(this.createIndexCommandSnapshot, this.createIndexCommand)
                        .ToString().Should().BeEmpty();
                }
            }
        }

        public class GetEventsSpec : DomainEventStoreSpec
        {
            private JObject command;

            [It(nameof(GetEvents))]
            public void Should_filter_events_older_than_provided_generation()
            {
                var generation = 42;
                var result = this.GetEvents(this.testTransEvent.Payload.AggregateId, generation);
                this.command.SelectToken("..range['Payload.Generation'].gt")
                    .Value<int>().Should().Be(generation);
            }

            [It(nameof(GetEvents))]
            public void Should_filter_events_by_provided_AggregateId()
            {
                var result = this.GetEvents(this.testTransEvent.Payload.AggregateId, 0);
                this.command.SelectToken("..term['Payload.AggregateId'].value")
                    .ToObject<Guid>().Should().Be(this.testTransEvent.Payload.AggregateId.Value);
            }

            protected override void OnRequestCompleted(IApiCallDetails x)
            {
                if (x.RequestBodyInBytes != null && x.RequestBodyInBytes.Length > 1)
                {
                    var body = Encoding.UTF8.GetString(x.RequestBodyInBytes);
                    this.command = JObject.Parse(body);
                }
            }
        }

        public class InitMappingSpec : DomainEventStoreSpec
        {
            protected override void OnRequestCompleted(IApiCallDetails x) { }

            [It(nameof(InitDefaultMapping))]
            public void Should_set_up_DefaultMappingFor_current_Event_type()
            {
                var cs = Substitute.For<ConnectionSettings>(new InMemoryConnection());

                this.InitDefaultMapping(cs);

                cs.ReceivedWithAnyArgs(1)
                    .DefaultMappingFor<ITransportMessage<DomainEvent<PublicSchemeId>, PublicSchemeId>>(map => map);
            }
        }

        public class SaveEventSpec : DomainEventStoreSpec
        {
            private ITransEvent resultTransEvent;

            [It(nameof(SaveEvent))]
            public async Task Should_correctly_serialize_Event()
            {
                var result = await this.SaveEvent(this.testTransEvent);

                this.resultTransEvent.CorrelationId.Should().Be(this.testTransEvent.CorrelationId);
                this.resultTransEvent.RequestTimestamp.Should().Be(this.testTransEvent.RequestTimestamp);
                this.resultTransEvent.UserId.Should().Be(this.testUserId);

                var testPayload = this.testTransEvent.Payload as SchemePublishedEvent;
                var resultPayload = this.resultTransEvent.Payload as SchemePublishedEvent;

                resultPayload.AggregateId.Should().Be(testPayload.AggregateId);
                resultPayload.Id.Should().Be(testPayload.Id);
                resultPayload.Generation.Should().Be(testPayload.Generation);
                resultPayload.ColorScheme.Should().Be(testPayload.ColorScheme);
            }

            protected override void OnRequestCompleted(IApiCallDetails x)
            {
                if (x.RequestBodyInBytes != null && x.RequestBodyInBytes.Length > 1)
                {
                    var body = Encoding.UTF8.GetString(x.RequestBodyInBytes);
                    var doc = JObject.Parse(body)["doc"].ToString();
                    var result = this.realMessageSerializer.Deserialize(doc);
                    result.HasError.Should().BeFalse(result.ErrorMessage);
                    this.resultTransEvent = result.Message;
                }
            }
        }
    }
}

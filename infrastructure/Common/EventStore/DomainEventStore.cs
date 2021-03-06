﻿using Elasticsearch.Net;
using MidnightLizard.Commons.Domain.Interfaces;
using MidnightLizard.Commons.Domain.Messaging;
using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Commons.Domain.Results;
using MidnightLizard.Impressions.Infrastructure.Configuration;
using MidnightLizard.Impressions.Infrastructure.Serialization.Common;
using Nest;
using System;
using System.Threading.Tasks;

namespace MidnightLizard.Impressions.Infrastructure.EventStore
{
    public abstract class DomainEventStore<TAggregateId> : IDomainEventStore<TAggregateId>
        where TAggregateId : DomainEntityId
    {
        protected readonly ElasticSearchConfig config;
        protected readonly IMessageSerializer messageSerializer;
        protected readonly ElasticClient elasticClient;

        public DomainEventStore(ElasticSearchConfig config, IMessageSerializer messageSerializer)
        {
            this.config = config;
            this.messageSerializer = messageSerializer;
            this.elasticClient = this.CreateElasticClient();
            this.CheckIndexExists();
        }

        protected virtual void CheckIndexExists()
        {
            if ((this.elasticClient.IndexExists(this.config.EventStoreIndexName)).Exists == false)
            {
                this.CreateIndex();
            }
        }

        protected virtual void CreateIndex()
        {
            this.elasticClient
                .CreateIndex(this.config.EventStoreIndexName, ix => ix
                    .Mappings(map => map
                        .Map<ITransportMessage<EventSourcedDomainEvent<TAggregateId>, TAggregateId>>(tm => tm
                            .RoutingField(x => x.Required())
                            .Properties(prop => prop
                                .Keyword(x => x.Name(nameof(Type)))
                                .Keyword(x => x.Name(nameof(Version)))
                                .Keyword(x => x.Name(n => n.CorrelationId))
                                .Date(x => x.Name(n => n.RequestTimestamp))
                                .Date(x => x.Name(n => n.EventTimestamp))
                                .Object<EventSourcedDomainEvent<TAggregateId>>(e => e
                                    .Name(x => x.Payload)
                                    .Properties(eProp => eProp
                                        .Keyword(x => x.Name(n => n.Id))
                                        .Keyword(x => x.Name(n => n.AggregateId))
                                        .Number(x => x
                                            .Name(n => n.Generation)
                                            .Type(NumberType.Integer)))))))
                    .Settings(set => set
                        .NumberOfShards(this.config.EventStoreShards)
                        .NumberOfReplicas(this.config.EventStoreReplicas)));
        }

        protected virtual ElasticClient CreateElasticClient()
        {
            var node = new Uri(this.config.ClientUrl);
            return new ElasticClient(this.InitDefaultMapping(new ConnectionSettings(
                new SingleNodeConnectionPool(node),
                (builtin, settings) => new DomainEventSerializer(this.messageSerializer))));
        }

        protected virtual ConnectionSettings InitDefaultMapping(ConnectionSettings connectionSettings)
        {
            return connectionSettings
                .DefaultFieldNameInferrer(i => i)
                .DefaultMappingFor<ITransportMessage<EventSourcedDomainEvent<TAggregateId>, TAggregateId>>(map => map
                     .IdProperty(to => to.Id)
                     .RoutingProperty(x => x.AggregateId)
                     .IndexName(this.config.EventStoreIndexName)
                     .TypeName("event"));
        }

        public async Task<DomainEventsResult<TAggregateId>> GetEvents(TAggregateId aggregateId, int sinceGeneration)
        {
            var results = await this.elasticClient.SearchAsync<ITransportMessage<EventSourcedDomainEvent<TAggregateId>, TAggregateId>>(s => s
               .Routing(aggregateId.ToString())
               .Sort(ss => ss.Ascending(x => x.Payload.Generation))
               .Query(q => q
                   .Bool(cs => cs
                       .Filter(f =>
                           f.Term(t => t.Payload.AggregateId, aggregateId) &&
                           f.Range(r => r
                               .Field(m => m.Payload.Generation)
                               .GreaterThan(sinceGeneration))))));
            if (results.IsValid)
            {
                return new DomainEventsResult<TAggregateId>(results.Documents);
            }
            return new DomainEventsResult<TAggregateId>(true, results.OriginalException, results.ServerError?.Error?.Reason);
        }

        public async Task<DomainResult> SaveEvent(ITransportMessage<EventSourcedDomainEvent<TAggregateId>> @event)
        {
            var result = await this.elasticClient.UpdateAsync<ITransportMessage<EventSourcedDomainEvent<TAggregateId>, TAggregateId>, object>(
                new DocumentPath<ITransportMessage<EventSourcedDomainEvent<TAggregateId>, TAggregateId>>(@event.Payload.Id),
                u => u
                    .Routing(@event.Payload.AggregateId.ToString())
                    .Doc(@event)
                    .DocAsUpsert());
            if (result.IsValid != true)
            {
                return new DomainResult(result.IsValid, result.OriginalException, result.ServerError?.Error?.Reason);
            }
            return DomainResult.Ok;
        }
    }
}

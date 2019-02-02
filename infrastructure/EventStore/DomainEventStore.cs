using Elasticsearch.Net;
using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Commons.Domain.Interfaces;
using MidnightLizard.Commons.Domain.Messaging;
using MidnightLizard.Commons.Domain.Results;
using MidnightLizard.Impressions.Domain.PublicSchemeAggregate;
using MidnightLizard.Impressions.Infrastructure.Configuration;
using MidnightLizard.Impressions.Infrastructure.Serialization.Common;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Impressions.Infrastructure.EventStore
{
    public abstract class DomainEventStore<TAggregateId> : IDomainEventStore<TAggregateId>
        where TAggregateId : DomainEntityId
    {
        protected readonly ElasticSearchConfig config;
        protected readonly IMessageSerializer messageSerializer;
        protected readonly ElasticClient elasticClient;
        protected abstract string IndexName { get; }

        public DomainEventStore(ElasticSearchConfig config, IMessageSerializer messageSerializer)
        {
            this.config = config;
            this.messageSerializer = messageSerializer;
            this.elasticClient = CreateElasticClient();
            CheckIndexExists();
        }

        protected virtual void CheckIndexExists()
        {
            if ((this.elasticClient.IndexExists(this.IndexName)).Exists == false)
            {
                CreateIndex();
            }
        }

        protected virtual void CreateIndex()
        {
            this.elasticClient
                .CreateIndex(this.IndexName, ix => ix
                    .Mappings(map => map
                        .Map<ITransportMessage<DomainEvent<TAggregateId>, TAggregateId>>(tm => tm
                            .RoutingField(x => x.Required())
                            .Properties(prop => prop
                                .Keyword(x => x.Name(nameof(Type)))
                                .Keyword(x => x.Name(nameof(Version)))
                                .Keyword(x => x.Name(n => n.CorrelationId))
                                .Date(x => x.Name(n => n.RequestTimestamp))
                                .Date(x => x.Name(n => n.EventTimestamp))
                                .Object<DomainEvent<TAggregateId>>(e => e
                                    .Name(x => x.Payload)
                                    .Properties(eProp => eProp
                                        .Keyword(x => x.Name(n => n.Id))
                                        .Keyword(x => x.Name(n => n.AggregateId))
                                        .Number(x => x
                                            .Name(n => n.Generation)
                                            .Type(NumberType.Integer)))))))
                    .Settings(set => set
                        .NumberOfShards(this.config.ELASTIC_SEARCH_EVENT_STORE_SHARDS)
                        .NumberOfReplicas(this.config.ELASTIC_SEARCH_EVENT_STORE_REPLICAS)));
        }

        protected virtual ElasticClient CreateElasticClient()
        {
            var node = new Uri(config.ELASTIC_SEARCH_CLIENT_URL);
            return new ElasticClient(InitDefaultMapping(new ConnectionSettings(
                new SingleNodeConnectionPool(node),
                (builtin, settings) => new DomainEventSerializer(messageSerializer))));
        }

        protected virtual ConnectionSettings InitDefaultMapping(ConnectionSettings connectionSettings)
        {
            return connectionSettings
                .DefaultFieldNameInferrer(i => i)
                .DefaultMappingFor<ITransportMessage<DomainEvent<TAggregateId>, TAggregateId>>(map => map
                     .IdProperty(to => to.Id)
                     .RoutingProperty(x => x.AggregateId)
                     .IndexName(this.IndexName)
                     .TypeName("event"));
        }

        public async Task<DomainEventsResult<TAggregateId>> GetEvents(TAggregateId aggregateId, int sinceGeneration)
        {
            var results = await this.elasticClient.SearchAsync<ITransportMessage<DomainEvent<TAggregateId>, TAggregateId>>(s => s
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

        public async Task<DomainResult> SaveEvent(ITransportMessage<DomainEvent<TAggregateId>> @event)
        {
            var result = await this.elasticClient.UpdateAsync<ITransportMessage<DomainEvent<TAggregateId>, TAggregateId>, object>(
                new DocumentPath<ITransportMessage<DomainEvent<TAggregateId>, TAggregateId>>(@event.Payload.Id),
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

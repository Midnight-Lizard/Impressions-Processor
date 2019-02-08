using Elasticsearch.Net;
using MidnightLizard.Commons.Domain.Interfaces;
using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Impressions.Infrastructure.Configuration;
using MidnightLizard.Impressions.Infrastructure.Serialization.Common;
using MidnightLizard.Impressions.Infrastructure.Versioning;
using Nest;
using System;
using System.Threading.Tasks;

namespace MidnightLizard.Impressions.Infrastructure.Snapshot
{
    public abstract class AggregateSnapshotAccessor<TAggregate, TAggregateId> : IAggregateSnapshotAccessor<TAggregate, TAggregateId>
        where TAggregateId : DomainEntityId
        where TAggregate : AggregateRoot<TAggregateId>
    {
        protected abstract string IndexName { get; }
        protected readonly IElasticClient elasticClient;
        protected readonly SchemaVersion version;
        protected readonly ElasticSearchConfig config;

        public AggregateSnapshotAccessor(SchemaVersion version, ElasticSearchConfig config)
        {
            this.version = version;
            this.config = config;
            this.elasticClient = this.CreateElasticClient();
            this.CheckIndexExists();
        }

        protected virtual void CheckIndexExists()
        {
            if ((this.elasticClient.IndexExists(this.IndexName)).Exists == false)
            {
                this.CreateIndex();
            }
        }

        protected virtual void CreateIndex()
        {
            this.elasticClient
                .CreateIndex(this.IndexName, ix => ix
                    .Mappings(this.ApplyAggregateMappingsOnIndex)
                    .Settings(set => set
                        .NumberOfShards(this.config.SnapshotShards)
                        .NumberOfReplicas(this.config.SnapshotReplicas)));
        }

        protected abstract IPromise<IMappings> ApplyAggregateMappingsOnIndex(MappingsDescriptor md);

        protected virtual IElasticClient CreateElasticClient()
        {
            var node = new Uri(this.config.ClientUrl);
            return new ElasticClient(this.ApplyAggregateMappingsOnConnection(new ConnectionSettings(
                new SingleNodeConnectionPool(node),
                (builtin, settings) => new AggregateSerializer())));
        }

        protected virtual ConnectionSettings ApplyAggregateMappingsOnConnection(ConnectionSettings connectionSettings)
        {
            return connectionSettings
               .DefaultFieldNameInferrer(i => i)
               .DefaultMappingFor<TAggregate>(map => map
                   .IdProperty(to => to.Id)
                   .IndexName(this.IndexName)
                   .TypeName("snapshot"));
        }

        public async Task<AggregateSnapshot<TAggregate, TAggregateId>> Read(TAggregateId id)
        {
            var result = await this.elasticClient
                .GetAsync<TAggregate>(new DocumentPath<TAggregate>(id.ToString()));

            if (result.IsValid &&
                result.Fields.Value<string>(nameof(Version)) == this.version.ToString())
            {
                var requestTimestampField = nameof(AggregateSnapshot<TAggregate, TAggregateId>.RequestTimestamp);

                return new AggregateSnapshot<TAggregate, TAggregateId>(result.Source,
                    result.Fields.Value<DateTime>(requestTimestampField));
            }
            return new AggregateSnapshot<TAggregate, TAggregateId>(this.CreateNewAggregate(id), DateTime.MinValue);
        }

        protected abstract TAggregate CreateNewAggregate(TAggregateId id);

        public abstract Task Save(AggregateSnapshot<TAggregate, TAggregateId> aggregate);
    }
}

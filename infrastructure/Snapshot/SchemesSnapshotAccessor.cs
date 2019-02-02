using System;
using System.Threading.Tasks;
using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Commons.Domain.Interfaces;
using MidnightLizard.Commons.Domain.Results;
using MidnightLizard.Impressions.Domain.PublicSchemeAggregate;
using MidnightLizard.Impressions.Infrastructure.Configuration;
using Nest;
using MidnightLizard.Impressions.Infrastructure.Versioning;

namespace MidnightLizard.Impressions.Infrastructure.Snapshot
{
    public class ImpressionsSnapshotAccessor : AggregateSnapshotAccessor<PublicScheme, PublicSchemeId>
    {
        protected override string IndexName => config.ELASTIC_SEARCH_SNAPSHOT_IMPRESSIONS_INDEX_NAME;
        protected override PublicScheme CreateNewAggregate(PublicSchemeId id) => new PublicScheme(id);

        public ImpressionsSnapshotAccessor(SchemaVersion version, ElasticSearchConfig config) : base(version, config)
        {
        }

        public override async Task Save(AggregateSnapshot<PublicScheme, PublicSchemeId> schemeSnapshot)
        {
            var result = await this.elasticClient.UpdateAsync<PublicScheme, object>(
                new DocumentPath<PublicScheme>(schemeSnapshot.Aggregate.Id.Value),
                u => u.Doc(new
                {
                    Version = version.ToString(),
                    schemeSnapshot.RequestTimestamp,
                    schemeSnapshot.Aggregate.PublisherId,
                    schemeSnapshot.Aggregate.ColorScheme,
                    schemeSnapshot.Aggregate.Description
                }).DocAsUpsert());
        }

        protected override IPromise<IMappings> ApplyAggregateMappingsOnIndex(MappingsDescriptor md)
        {
            return md.Map<PublicScheme>(tm => tm
                .Properties(prop => prop
                    .Keyword(x => x.Name(nameof(Version)))
                    .Date(x => x.Name(nameof(AggregateSnapshot<PublicScheme, PublicSchemeId>.RequestTimestamp)))
                    .Keyword(x => x.Name(n => n.PublisherId))
                    .Keyword(x => x.Name(n => n.Description))
                    .Object<ColorScheme>(cs => cs
                        .Name(x => x.ColorScheme)
                        .AutoMap()
                        .Properties(eProp => eProp
                            .Keyword(x => x.Name(n => n.colorSchemeId))
                            .Keyword(x => x.Name(n => n.colorSchemeName))))));
        }
    }
}
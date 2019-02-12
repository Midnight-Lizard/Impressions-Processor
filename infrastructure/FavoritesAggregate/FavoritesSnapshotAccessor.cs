using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Impressions.Domain.FavoritesAggregate;
using MidnightLizard.Impressions.Infrastructure.Configuration;
using MidnightLizard.Impressions.Infrastructure.Snapshot;
using MidnightLizard.Impressions.Infrastructure.Versioning;
using Nest;
using System;
using System.Threading.Tasks;

namespace MidnightLizard.Impressions.Infrastructure.FavoritesAggregate
{
    public class FavoritesSnapshotAccessor : AggregateSnapshotAccessor<Favorites, FavoritesId>
    {
        public FavoritesSnapshotAccessor(SchemaVersion version, FavoritesElasticSearchConfig config)
            : base(version.ToString(), config)
        {
        }

        protected override Favorites CreateNewAggregate(FavoritesId id)
        {
            return new Favorites(id);
        }

        protected override IPromise<IMappings> ApplyAggregateMappingsOnIndex(MappingsDescriptor md)
        {
            return md.Map<Favorites>(tm => tm
                .Properties(prop => prop
                    .Keyword(x => x.Name(nameof(Version)))
                    .Date(x => x.Name(nameof(AggregateSnapshot<Favorites, FavoritesId>.RequestTimestamp)))
                    .Keyword(x => x.Name(n => n.ObjectType))
                    .Keyword(x => x.Name(n => n.FavoritedBy))));
        }

        public override Task Save(AggregateSnapshot<Favorites, FavoritesId> snapshot)
        {
            return this.elasticClient.UpdateAsync<Favorites, object>(
                new DocumentPath<Favorites>(snapshot.Aggregate.Id.Value),
                u => u.Doc(new
                {
                    Version = this.schemaVersion,
                    snapshot.RequestTimestamp,
                    snapshot.Aggregate.ObjectType,
                    snapshot.Aggregate.FavoritedBy
                }).DocAsUpsert());
        }
    }
}

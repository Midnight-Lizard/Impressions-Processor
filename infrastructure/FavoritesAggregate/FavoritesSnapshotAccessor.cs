using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Impressions.Domain.FavoritesAggregate;
using MidnightLizard.Impressions.Domain.ImpressionsAggregate;
using MidnightLizard.Impressions.Domain.LikesAggregate;
using MidnightLizard.Impressions.Infrastructure.Configuration;
using MidnightLizard.Impressions.Infrastructure.Snapshot;
using MidnightLizard.Impressions.Infrastructure.Versioning;
using Nest;
using System;
using System.Threading.Tasks;

namespace MidnightLizard.Impressions.Infrastructure.FavoritesAggregate
{
    public class FavoritesSnapshotAccessor : AggregateSnapshotAccessor<Favorites, ImpressionsObjectId>
    {
        public FavoritesSnapshotAccessor(SchemaVersion version, ElasticSearchConfig config)
            : base(version.ToString(), config)
        {
        }

        protected override Favorites CreateNewAggregate(ImpressionsObjectId id)
        {
            return new Favorites(id);
        }

        protected override IPromise<IMappings> ApplyAggregateMappingsOnIndex(MappingsDescriptor md)
        {
            return md.Map<Favorites>(tm => tm
                .Properties(prop => prop
                    .Keyword(x => x.Name(nameof(Version)))
                    .Date(x => x.Name(nameof(AggregateSnapshot<Likes, ImpressionsObjectId>.RequestTimestamp)))
                    .Keyword(x => x.Name(n => n.ObjectType))
                    .Keyword(x => x.Name(n => n.FavoritedBy))));
        }

        public override Task Save(AggregateSnapshot<Favorites, ImpressionsObjectId> snapshot)
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

﻿using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Impressions.Domain.ImpressionsAggregate;
using MidnightLizard.Impressions.Domain.LikesAggregate;
using MidnightLizard.Impressions.Infrastructure.Configuration;
using MidnightLizard.Impressions.Infrastructure.Snapshot;
using MidnightLizard.Impressions.Infrastructure.Versioning;
using Nest;
using System;
using System.Threading.Tasks;

namespace MidnightLizard.Impressions.Infrastructure.LikesAggregate
{
    public class LikesSnapshotAccessor : AggregateSnapshotAccessor<Likes, ImpressionsObjectId>
    {
        public LikesSnapshotAccessor(SchemaVersion version, ElasticSearchConfig config)
            : base(version.ToString(), config)
        {
        }

        protected override Likes CreateNewAggregate(ImpressionsObjectId id)
        {
            return new Likes(id);
        }

        protected override IPromise<IMappings> ApplyAggregateMappingsOnIndex(MappingsDescriptor md)
        {
            return md.Map<Likes>(tm => tm
                .Properties(prop => prop
                    .Keyword(x => x.Name(nameof(Version)))
                    .Date(x => x.Name(nameof(AggregateSnapshot<Likes, ImpressionsObjectId>.RequestTimestamp)))
                    .Keyword(x => x.Name(n => n.ObjectType))
                    .Keyword(x => x.Name(n => n.LikedBy))));
        }

        public override Task Save(AggregateSnapshot<Likes, ImpressionsObjectId> snapshot)
        {
            return this.elasticClient.UpdateAsync<Likes, object>(
                new DocumentPath<Likes>(snapshot.Aggregate.Id.Value),
                u => u.Doc(new
                {
                    Version = this.schemaVersion,
                    snapshot.RequestTimestamp,
                    snapshot.Aggregate.ObjectType,
                    snapshot.Aggregate.LikedBy
                }).DocAsUpsert());
        }
    }
}

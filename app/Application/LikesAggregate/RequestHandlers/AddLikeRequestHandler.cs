using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MidnightLizard.Commons.Domain.Interfaces;
using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Impressions.Domain.LikesAggregate;
using MidnightLizard.Impressions.Domain.LikesAggregate.Requests;
using MidnightLizard.Impressions.Processor.Application.DomainRequestHandlers;
using MidnightLizard.Impressions.Processor.Configuration;
using System.Threading;

namespace MidnightLizard.Impressions.Processor.Application.LikesAggregate.RequestHandlers
{
    public class AddLikeRequestHandler :
        DomainRequestHandler<Likes, AddLikeRequest, LikesId>
    {
        public AddLikeRequestHandler(
            IOptions<AggregatesConfig> cacheConfig,
            IMemoryCache memoryCache,
            IDomainEventDispatcher<LikesId> domainEventsDispatcher,
            IAggregateSnapshotAccessor<Likes, LikesId> snapshotAccessor,
            IDomainEventStore<LikesId> eventsAccessor) :
            base(cacheConfig, memoryCache, domainEventsDispatcher, snapshotAccessor, eventsAccessor)
        {
        }

        protected override void HandleDomainRequest(Likes aggregate, AddLikeRequest request, UserId userId, CancellationToken cancellationToken)
        {
            aggregate.AddImpression(userId, request.ObjectType);
        }
    }
}

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MidnightLizard.Commons.Domain.Interfaces;
using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Impressions.Domain.FavoritesAggregate;
using MidnightLizard.Impressions.Domain.FavoritesAggregate.Requests;
using MidnightLizard.Impressions.Processor.Application.DomainRequestHandlers;
using MidnightLizard.Impressions.Processor.Configuration;
using System.Threading;

namespace MidnightLizard.Impressions.Processor.Application.FavoritesAggregate.RequestHandlers
{
    public class AddToFavoritesRequestHandler :
        DomainRequestHandler<Favorites, AddToFavoritesRequest, FavoritesId>
    {
        public AddToFavoritesRequestHandler(
            IOptions<AggregatesConfig> cacheConfig,
            IMemoryCache memoryCache,
            IDomainEventDispatcher<FavoritesId> domainEventsDispatcher,
            IAggregateSnapshotAccessor<Favorites, FavoritesId> snapshotAccessor,
            IDomainEventStore<FavoritesId> eventsAccessor) :
            base(cacheConfig, memoryCache, domainEventsDispatcher, snapshotAccessor, eventsAccessor)
        {
        }

        protected override void HandleDomainRequest(Favorites aggregate, AddToFavoritesRequest request, UserId userId, CancellationToken cancellationToken)
        {
            aggregate.AddImpression(userId, request.ObjectType);
        }
    }
}

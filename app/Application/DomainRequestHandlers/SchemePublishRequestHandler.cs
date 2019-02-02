using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Commons.Domain.Interfaces;
using MidnightLizard.Commons.Domain.Messaging;
using MidnightLizard.Commons.Domain.Results;
using MidnightLizard.Impressions.Domain.PublicSchemeAggregate;
using MidnightLizard.Impressions.Domain.PublicSchemeAggregate.Requests;
using MidnightLizard.Impressions.Processor.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MidnightLizard.Impressions.Processor.Application.DomainRequestHandlers
{
    public class SchemePublishRequestHandler :
        DomainRequestHandler<PublicScheme, PublishSchemeRequest, PublicSchemeId>
    {
        public SchemePublishRequestHandler(
            IOptions<AggregatesConfig> cacheConfig,
            IMemoryCache memoryCache,
            IDomainEventDispatcher<PublicSchemeId> domainEventsDispatcher,
            IAggregateSnapshotAccessor<PublicScheme, PublicSchemeId> impressionsSnapshot,
            IDomainEventStore<PublicSchemeId> eventsAccessor) :
            base(cacheConfig, memoryCache, domainEventsDispatcher, impressionsSnapshot, eventsAccessor)
        {
        }

        protected override void HandleDomainRequest(PublicScheme aggregate, PublishSchemeRequest request, UserId userId, CancellationToken cancellationToken)
        {
            aggregate.Publish(userId, request.ColorScheme, request.Description);
        }
    }
}

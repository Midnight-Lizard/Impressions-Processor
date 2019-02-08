using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MidnightLizard.Commons.Domain.Interfaces;
using MidnightLizard.Commons.Domain.Messaging;
using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Commons.Domain.Results;
using MidnightLizard.Impressions.Processor.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MidnightLizard.Impressions.Processor.Application.DomainRequestHandlers
{
    public abstract class DomainRequestHandler<TAggregate, TRequest, TAggregateId> :
        IRequestHandler<TransportMessage<TRequest, TAggregateId>, DomainResult>
        where TRequest : DomainRequest<TAggregateId>
        where TAggregate : AggregateRoot<TAggregateId>
        where TAggregateId : DomainEntityId
    {
        protected readonly IOptions<AggregatesConfig> aggregatesConfig;
        protected readonly IMemoryCache memoryCache;
        protected readonly IDomainEventDispatcher<TAggregateId> eventsDispatcher;
        protected readonly IAggregateSnapshotAccessor<TAggregate, TAggregateId> aggregateSnapshotAccessor;
        protected readonly IDomainEventStore<TAggregateId> eventStrore;

        protected DomainRequestHandler(
            IOptions<AggregatesConfig> aggConfig,
            IMemoryCache memoryCache,
            IDomainEventDispatcher<TAggregateId> eventsDispatcher,
            IAggregateSnapshotAccessor<TAggregate, TAggregateId> aggregateSnapshot,
            IDomainEventStore<TAggregateId> eventStrore)
        {
            this.aggregatesConfig = aggConfig;
            this.memoryCache = memoryCache;
            this.eventsDispatcher = eventsDispatcher;
            this.aggregateSnapshotAccessor = aggregateSnapshot;
            this.eventStrore = eventStrore;
        }

        protected abstract void
            HandleDomainRequest(TAggregate aggregate, TRequest request, UserId userId, CancellationToken cancellationToken);

        public virtual async Task<DomainResult>
            Handle(TransportMessage<TRequest, TAggregateId> transRequest, CancellationToken cancellationToken
            )
        {
            var someResult = await this.GetAggregate(transRequest.Payload.AggregateId);
            if (someResult.HasError)
            {
                return someResult;
            }

            if (someResult is AggregateSnapshotResult<TAggregate, TAggregateId> aggregateSnapshotResult)
            {
                var aggregateSnapshot = aggregateSnapshotResult.AggregateSnapshot;

                if (aggregateSnapshot.Aggregate.IsNew() || aggregateSnapshot.RequestTimestamp < transRequest.RequestTimestamp)
                {
                    this.HandleDomainRequest(aggregateSnapshot.Aggregate, transRequest.Payload, transRequest.UserId, cancellationToken);

                    var dispatchResults = await this.DispatchDomainEvents(aggregateSnapshot.Aggregate, transRequest);
                    var error = dispatchResults.Values.FirstOrDefault(result => result.HasError);
                    if (error != null)
                    {
                        if (this.aggregatesConfig.Value.AGGREGATE_CACHE_ENABLED)
                        {
                            this.memoryCache.Remove(aggregateSnapshot.Aggregate.Id);
                        }
                        return error;
                    }
                    else
                    {
                        // in case this aggregate will stay in memory until next request
                        aggregateSnapshot.RequestTimestamp = transRequest.RequestTimestamp;
                    }
                }
                return DomainResult.Ok;
            }
            return new DomainResult($"{nameof(GetAggregate)} returned a wrong type of result: {someResult?.GetType().FullName ?? "null"}");
        }

        protected virtual async Task<AggregateSnapshot<TAggregate, TAggregateId>>
            GetAggregateSnapshot(TAggregateId id
            )
        {
            if (this.aggregatesConfig.Value.AGGREGATE_CACHE_ENABLED &&
                this.memoryCache.TryGetValue(id, out AggregateSnapshot<TAggregate, TAggregateId> aggregateSnapshot))
            {
                return aggregateSnapshot;
            }
            else
            {
                aggregateSnapshot = await this.aggregateSnapshotAccessor.Read(id);
                if (this.aggregatesConfig.Value.AGGREGATE_CACHE_ENABLED)
                {
                    this.memoryCache.Set(id, aggregateSnapshot, new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromSeconds(this.aggregatesConfig.Value.AGGREGATE_CACHE_SLIDING_EXPIRATION_SECONDS))
                        .SetAbsoluteExpiration(DateTimeOffset.Now.AddSeconds(this.aggregatesConfig.Value.AGGREGATE_CACHE_ABSOLUTE_EXPIRATION_SECONDS)));
                }
                return aggregateSnapshot;
            }
        }

        protected virtual async Task<Dictionary<DomainEvent<TAggregateId>, DomainResult>>
            DispatchDomainEvents(IEventSourced<TAggregateId> aggregate, TransportMessage<TRequest, TAggregateId> transRequest
            )
        {
            var hasError = false;
            var results = new Dictionary<DomainEvent<TAggregateId>, DomainResult>();
            foreach (var @event in aggregate.ReleaseEvents())
            {
                if (!hasError)
                {
                    var result = await this.eventsDispatcher.DispatchEvent(
                        new TransportMessage<DomainEvent<TAggregateId>, TAggregateId>(
                            @event, transRequest.CorrelationId, transRequest.UserId, transRequest.RequestTimestamp));
                    results.Add(@event, result);
                    if (result.HasError)
                    {
                        hasError = true;
                    }
                }
                else
                {
                    results.Add(@event, DomainResult.Skipped);
                }
            }
            return results;
        }

        protected virtual async Task<DomainResult>
            GetAggregate(TAggregateId aggregateId
            )
        {
            var aggregateSnapshot = await this.GetAggregateSnapshot(aggregateId);

            var eventsResult = await this.eventStrore.GetEvents(aggregateId, aggregateSnapshot.Aggregate.Generation);
            if (eventsResult.HasError)
            {
                return eventsResult;
            }

            if (eventsResult.Events.Count() > 0)
            {
                aggregateSnapshot.Aggregate.ReplayEventSourcedDomainEvents(eventsResult.Events.Select(x => (x.Payload, x.UserId)));
                aggregateSnapshot.RequestTimestamp = eventsResult.Events.Max(e => e.RequestTimestamp);
            }

            if (eventsResult.Events.Count() > this.aggregatesConfig.Value.AGGREGATE_MAX_EVENTS_COUNT && !aggregateSnapshot.Aggregate.IsNew())
            {
                await this.aggregateSnapshotAccessor.Save(aggregateSnapshot);
            }

            return new AggregateSnapshotResult<TAggregate, TAggregateId>(aggregateSnapshot);
        }
    }
}

using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MidnightLizard.Commons.Domain.Interfaces;
using MidnightLizard.Commons.Domain.Messaging;
using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Commons.Domain.Results;
using MidnightLizard.Impressions.Domain.PublicSchemeAggregate;
using MidnightLizard.Impressions.Domain.PublicSchemeAggregate.Events;
using MidnightLizard.Impressions.Domain.PublicSchemeAggregate.Requests;
using MidnightLizard.Impressions.Processor.Configuration;
using MidnightLizard.Testing.Utilities;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MidnightLizard.Impressions.Processor.Application.DomainRequestHandlers
{
    public class DomainRequestHandlerSpec : DomainRequestHandler<PublicScheme, DomainRequest<PublicSchemeId>, PublicSchemeId>
    {
        private int handleDomainRequest_CallCount = 0;

        private readonly List<DomainEvent<PublicSchemeId>> testEvents;
        private readonly IEnumerable<TransportMessage<DomainEvent<PublicSchemeId>, PublicSchemeId>> testTransEvents;
        private readonly AggregatesConfig testCacheConfig =
            new AggregatesConfig
            {
                AGGREGATE_CACHE_ENABLED = true,
                AGGREGATE_CACHE_SLIDING_EXPIRATION_SECONDS = 10,
                AGGREGATE_CACHE_ABSOLUTE_EXPIRATION_SECONDS = 60,
                AGGREGATE_MAX_EVENTS_COUNT = 1
            };
        private readonly DateTime testRequestTimestamp = DateTime.Now;
        private readonly PublicScheme testScheme = Substitute.For<PublicScheme>();
        private readonly AggregateSnapshot<PublicScheme, PublicSchemeId> testSchemeSnapshot;
        private readonly PublishSchemeRequest testRequest = Substitute.For<PublishSchemeRequest>();
        private readonly ICacheEntry cacheEntry = Substitute.For<ICacheEntry>();
        private readonly TransportMessage<DomainRequest<PublicSchemeId>, PublicSchemeId> testTransRequest;
        private readonly UserId testPublisherId = new UserId("test-user-id");

        public DomainRequestHandlerSpec() : base(
            Substitute.For<IOptions<AggregatesConfig>>(),
            Substitute.For<IMemoryCache>(),
            Substitute.For<IDomainEventDispatcher<PublicSchemeId>>(),
            Substitute.For<IAggregateSnapshotAccessor<PublicScheme, PublicSchemeId>>(),
            Substitute.For<IDomainEventStore<PublicSchemeId>>())
        {
            this.testScheme.Id.Returns(new PublicSchemeId());
            this.testSchemeSnapshot = new AggregateSnapshot<PublicScheme, PublicSchemeId>(
                this.testScheme, DateTime.MinValue);
            this.testTransRequest = new TransportMessage<DomainRequest<PublicSchemeId>, PublicSchemeId>(
                this.testRequest, Guid.NewGuid(), this.testPublisherId, this.testRequestTimestamp, this.testRequestTimestamp);

            this.testEvents = new List<DomainEvent<PublicSchemeId>>
            {
                new SchemePublishedEvent(this.testScheme.Id, new ColorScheme(), null),
                new SchemePublishedEvent(this.testScheme.Id, new ColorScheme(), null)
            };

            this.testTransEvents = this.testEvents.Select(e =>
                new TransportMessage<DomainEvent<PublicSchemeId>, PublicSchemeId>(
                    e, Guid.NewGuid(), this.testPublisherId, DateTime.MinValue, DateTime.MinValue));

            this.eventStrore.ClearReceivedCalls();
            this.aggregateSnapshotAccessor.ClearReceivedCalls();
            this.eventsDispatcher.ClearReceivedCalls();
            this.memoryCache.ClearReceivedCalls();
            this.aggregatesConfig.ClearReceivedCalls();
        }

        protected override void HandleDomainRequest(PublicScheme aggregate, DomainRequest<PublicSchemeId> request, UserId userId, CancellationToken cancellationToken
            )
        {
            aggregate.Should().BeSameAs(this.testScheme);
            request.AggregateId.Should().BeSameAs(aggregate.Id);

            this.handleDomainRequest_CallCount++;
        }

        public class DispatchDomainEventsSpec : DomainRequestHandlerSpec
        {
            public DispatchDomainEventsSpec() : base()
            {
                this.testScheme.ReleaseEvents()
                    .Returns(this.testEvents);

                this.eventsDispatcher.DispatchEvent(Arg.Any<TransportMessage<DomainEvent<PublicSchemeId>, PublicSchemeId>>())
                    .Returns(DomainResult.Ok);
            }

            [It(nameof(DispatchDomainEvents))]
            public async Task Should_call_Aggregate__ReleaseEvents(
                )
            {
                var results = await this.DispatchDomainEvents(this.testScheme, this.testTransRequest);

                this.testScheme.Received(1).ReleaseEvents();
            }

            [It(nameof(DispatchDomainEvents))]
            public async Task Should_return_successful_results(
                )
            {
                var results = await this.DispatchDomainEvents(this.testScheme, this.testTransRequest);

                results.Values.Should().NotContain(val => val.HasError);
            }

            [It(nameof(DispatchDomainEvents))]
            public async Task Should_return_results_for_each_Event_returned_from_Aggregate__ReleaseEvents(
                )
            {
                var results = await this.DispatchDomainEvents(this.testScheme, this.testTransRequest);

                results.Should().HaveCount(this.testEvents.Count);
            }

            [It(nameof(DispatchDomainEvents))]
            public async Task Should_call_DispatchEvent_for_each_Event_returned_from_Aggregate__ReleaseEvents(
                )
            {
                var results = await this.DispatchDomainEvents(this.testScheme, this.testTransRequest);

                await this.eventsDispatcher
                    .Received(this.testEvents.Count)
                    .DispatchEvent(Arg.Any<TransportMessage<DomainEvent<PublicSchemeId>, PublicSchemeId>>());
            }

            [It(nameof(DispatchDomainEvents))]
            public async Task Should_stop_dispatching_events_if_error_returned_by_EventsDispatcher(
               )
            {
                this.eventsDispatcher
                    .DispatchEvent(Arg.Any<TransportMessage<DomainEvent<PublicSchemeId>, PublicSchemeId>>())
                    .Returns(DomainResult.UnknownError);

                var results = await this.DispatchDomainEvents(this.testScheme, this.testTransRequest);

                results.Values.Should().ContainSingle(r => r.HasError);
                await this.eventsDispatcher.Received(1)
                    .DispatchEvent(Arg.Any<TransportMessage<DomainEvent<PublicSchemeId>, PublicSchemeId>>());
            }
        }

        public class GetAggregateSnapshotSpec : DomainRequestHandlerSpec
        {
            private AggregateSnapshot<PublicScheme, PublicSchemeId> snapshotReadResult;

            public GetAggregateSnapshotSpec() : base()
            {
                this.snapshotReadResult = this.testSchemeSnapshot;

                this.memoryCache.TryGetValue(this.testScheme.Id, out var some)
                    .Returns(false);

                this.memoryCache.CreateEntry(this.testScheme.Id)
                    .Returns(this.cacheEntry);

                this.aggregateSnapshotAccessor.Read(this.testScheme.Id)
                    .Returns(x => this.snapshotReadResult);

                this.aggregatesConfig.Value
                    .Returns(this.testCacheConfig);
            }

            [It(nameof(GetAggregateSnapshot))]
            public async Task Should_return_AggregateResult_from_Snapshot(
                )
            {
                var result = await this.GetAggregateSnapshot(this.testScheme.Id);

                result.Should().BeSameAs(this.snapshotReadResult);
            }

            [It(nameof(GetAggregateSnapshot))]
            public async Task Should_call_AggregateSnapshot__Read_with_specified_AggregateId(
                )
            {
                var result = await this.GetAggregateSnapshot(this.testScheme.Id);

                await this.aggregateSnapshotAccessor.Received(1).Read(this.testScheme.Id);
            }

            [It(nameof(GetAggregateSnapshot))]
            public async Task Should_create_a_new_MemoryCacheEntry_for_specified_AggregateId(
                )
            {
                var result = await this.GetAggregateSnapshot(this.testScheme.Id);

                this.memoryCache.Received(1).CreateEntry(this.testScheme.Id);
            }

            [It(nameof(GetAggregateSnapshot))]
            public async Task Should_set_up_correct_SlidingExpiration_for_a_new_MemoryCacheEntry(
                )
            {
                var result = await this.GetAggregateSnapshot(this.testScheme.Id);

                this.cacheEntry.Received(1).SlidingExpiration =
                    TimeSpan.FromSeconds(this.testCacheConfig.AGGREGATE_CACHE_SLIDING_EXPIRATION_SECONDS);
            }

            [It(nameof(GetAggregateSnapshot))]
            public async Task Should_set_up_correct_AbsoluteExpiration_for_a_new_MemoryCacheEntry(
                )
            {
                var result = await this.GetAggregateSnapshot(this.testScheme.Id);

                var now = DateTimeOffset.Now;

                this.cacheEntry.Received(1).AbsoluteExpiration =
                    Arg.Is<DateTimeOffset>(dt => dt > now &&
                        dt < now.AddSeconds(this.testCacheConfig.AGGREGATE_CACHE_ABSOLUTE_EXPIRATION_SECONDS));
            }

            [It(nameof(GetAggregateSnapshot))]
            public async Task Should_not_read_from_MemoryCache_if_it_is_disabled(
                )
            {
                this.testCacheConfig.AGGREGATE_CACHE_ENABLED = false;

                var result = await this.GetAggregateSnapshot(this.testScheme.Id);

                this.memoryCache.DidNotReceiveWithAnyArgs().TryGetValue(this.testScheme.Id, out var some);
            }

            [It(nameof(GetAggregateSnapshot))]
            public async Task Should_not_write_to_MemoryCache_if_it_is_disabled(
                )
            {
                this.testCacheConfig.AGGREGATE_CACHE_ENABLED = false;

                var result = await this.GetAggregateSnapshot(this.testScheme.Id);

                this.memoryCache.DidNotReceiveWithAnyArgs().CreateEntry(null);
            }
        }

        public class GetAggregateSpec : DomainRequestHandlerSpec
        {
            private DomainEventsResult<PublicSchemeId> eventsReadResult;

            public GetAggregateSpec() : base()
            {
                this.eventsReadResult = new DomainEventsResult<PublicSchemeId>(this.testTransEvents);

                this.aggregatesConfig.Value
                    .Returns(this.testCacheConfig);

                this.aggregateSnapshotAccessor.Read(this.testScheme.Id)
                    .Returns(x => this.testSchemeSnapshot);

                this.eventStrore.GetEvents(this.testScheme.Id, 0)
                    .Returns(x => this.eventsReadResult);

                this.memoryCache.CreateEntry(this.testScheme.Id)
                    .Returns(this.cacheEntry);
            }

            [It(nameof(GetAggregate))]
            public async Task Should_return_Error_when_ReadDomainEvents_returns_Error(
                )
            {
                this.eventsReadResult = new DomainEventsResult<PublicSchemeId>("error");

                var result = await this.GetAggregate(this.testScheme.Id);

                result.Should().BeSameAs(this.eventsReadResult);
            }

            [It(nameof(GetAggregate))]
            public async Task Should_call_AggregateSnapshot__Read(
                )
            {
                var result = await this.GetAggregate(this.testScheme.Id);

                await this.aggregateSnapshotAccessor.Received(1).Read(this.testScheme.Id);
            }

            [It(nameof(GetAggregate))]
            public async Task Should_call_Aggregate__ReplayDomainEvents(
                )
            {
                var result = await this.GetAggregate(this.testScheme.Id);

                this.testScheme.Received(1).ReplayDomainEvents(Arg.Is<IEnumerable<(DomainEvent<PublicSchemeId>, UserId)>>(events =>
                     events.All(e => this.testEvents.Contains(e.Item1))));
            }

            [It(nameof(GetAggregate))]
            public async Task Should_save_AggregateSnapshot_if_it_has_too_many_Events(
                )
            {
                var result = await this.GetAggregate(this.testScheme.Id);

                await this.aggregateSnapshotAccessor.Received(1).Save(this.testSchemeSnapshot);
            }

            [It(nameof(GetAggregate))]
            public async Task Should_not_save_AggregateSnapshot_if_Aggregate_IsNew(
                )
            {
                this.testScheme.IsNew().Returns(true);

                var result = await this.GetAggregate(this.testScheme.Id);

                await this.aggregateSnapshotAccessor.DidNotReceiveWithAnyArgs().Save(this.testSchemeSnapshot);
            }
        }

        public class HandleSpec : DomainRequestHandlerSpec
        {
            private DomainResult dispatchEventResult = DomainResult.Ok;
            private int dispatchDomainEvents_CallCount = 0;
            private bool returnErrorFromGetAggregate = false;

            protected override Task<Dictionary<DomainEvent<PublicSchemeId>, DomainResult>> DispatchDomainEvents(
               IEventSourced<PublicSchemeId> aggregate, TransportMessage<DomainRequest<PublicSchemeId>, PublicSchemeId> transRequest)
            {
                aggregate.Should().BeSameAs(this.testScheme);
                this.dispatchDomainEvents_CallCount++;
                return base.DispatchDomainEvents(aggregate, transRequest);
            }

            protected override async Task<DomainResult> GetAggregate(PublicSchemeId aggregateId)
            {
                if (this.returnErrorFromGetAggregate)
                {
                    return DomainResult.UnknownError;
                }

                return await base.GetAggregate(aggregateId);
            }

            public HandleSpec() : base()
            {
                this.testScheme.ReleaseEvents()
                    .Returns(this.testEvents);

                this.testRequest.AggregateId.Returns(x => this.testScheme.Id);

                this.eventsDispatcher.DispatchEvent(Arg.Any<TransportMessage<DomainEvent<PublicSchemeId>, PublicSchemeId>>())
                    .Returns(x => this.dispatchEventResult);

                this.aggregatesConfig.Value
                    .Returns(this.testCacheConfig);

                this.memoryCache.CreateEntry(this.testScheme.Id)
                    .Returns(this.cacheEntry);

                this.eventStrore.GetEvents(this.testScheme.Id, 0)
                    .Returns(new DomainEventsResult<PublicSchemeId>(this.testTransEvents));

                this.aggregateSnapshotAccessor.Read(this.testScheme.Id)
                    .Returns(this.testSchemeSnapshot);
            }

            [It(nameof(Handle))]
            public async Task Should_return_Error_if_GetAggregate_returns_Error(
                )
            {
                this.returnErrorFromGetAggregate = true;

                var result = await this.Handle(this.testTransRequest, new CancellationToken());

                result.Should().BeSameAs(DomainResult.UnknownError);
            }

            [It(nameof(Handle))]
            public async Task Should_call_HandleDomainRequest(
                )
            {
                var result = await this.Handle(this.testTransRequest, new CancellationToken());

                this.handleDomainRequest_CallCount.Should().Be(1);
            }

            [It(nameof(Handle))]
            public async Task Should_call_DispatchDomainEvents(
                )
            {
                var result = await this.Handle(this.testTransRequest, new CancellationToken());

                this.dispatchDomainEvents_CallCount.Should().Be(1);
            }

            [It(nameof(Handle))]
            public async Task Should_return_Error_from_DispatchDomainEvents(
                )
            {
                this.dispatchEventResult = DomainResult.UnknownError;

                var result = await this.Handle(this.testTransRequest, new CancellationToken());

                result.Should().BeSameAs(this.dispatchEventResult);
            }

            [It(nameof(Handle))]
            public async Task Should_remove_Aggregate_from_MemoryCache_if_failed_to_DispatchEvent(
                )
            {
                this.dispatchEventResult = DomainResult.UnknownError;

                var result = await this.Handle(this.testTransRequest, new CancellationToken());

                this.memoryCache.Received(1).Remove(this.testScheme.Id);
            }

            [It(nameof(Handle))]
            public async Task Should_copy_RequestTimestamp_from_Request_to_AggregateSnapshot(
                )
            {
                var result = await this.Handle(this.testTransRequest, new CancellationToken());

                this.testSchemeSnapshot.RequestTimestamp.Should().Be(this.testRequestTimestamp);
            }

            [It(nameof(Handle))]
            public async Task Should_not_update_AggregateSnapshot__RequestTimastamp_if_GetAggregate_returns_Error(
                )
            {
                this.returnErrorFromGetAggregate = true;

                var result = await this.Handle(this.testTransRequest, new CancellationToken());

                this.testSchemeSnapshot.RequestTimestamp.Should().NotBe(this.testRequestTimestamp);
                this.testSchemeSnapshot.RequestTimestamp.Should().Be(DateTime.MinValue);
            }

            [It(nameof(Handle))]
            public async Task Should_not_proceed_if_Request__Timestamp_is_not_older_than_AggregateSnapshot__RequestTimastamp(
                )
            {
                this.testSchemeSnapshot.RequestTimestamp = DateTime.Now;
                this.eventStrore.GetEvents(this.testScheme.Id, 0)
                   .Returns(new DomainEventsResult<PublicSchemeId>(
                       new List<TransportMessage<DomainEvent<PublicSchemeId>, PublicSchemeId>>()));

                var result = await this.Handle(this.testTransRequest, new CancellationToken());

                this.handleDomainRequest_CallCount.Should().Be(0);
            }
        }
    }
}

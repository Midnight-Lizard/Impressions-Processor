using FluentAssertions;
using MidnightLizard.Commons.Domain.Interfaces;
using MidnightLizard.Commons.Domain.Messaging;
using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Commons.Domain.Results;
using MidnightLizard.Impressions.Domain.PublicSchemeAggregate;
using MidnightLizard.Impressions.Domain.PublicSchemeAggregate.Events;
using MidnightLizard.Testing.Utilities;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MidnightLizard.Impressions.Processor.Application.DomainEventHandlers
{
    public class DomainEventHandlerSpec : DomainEventHandler<SchemePublishedEvent, PublicSchemeId>
    {
        private readonly TransportMessage<SchemePublishedEvent, PublicSchemeId> testTransEvent;
        private readonly DomainResult testResult = new DomainResult("test");

        public DomainEventHandlerSpec() : base(Substitute.For<IDomainEventStore<PublicSchemeId>>())
        {
            this.testTransEvent = new TransportMessage<SchemePublishedEvent, PublicSchemeId>(null, new Guid(), new UserId("test-user-id"), DateTime.UtcNow, DateTime.UtcNow);
            this.domainEventAccessor.SaveEvent(this.testTransEvent).Returns(this.testResult);
        }

        [It(nameof(Handle))]
        public async Task Should_call_DomainEventAccessor__SaveEvent()
        {
            var result = await this.Handle(this.testTransEvent, new CancellationToken());

            await this.domainEventAccessor.Received(1).SaveEvent(this.testTransEvent);
        }

        [It(nameof(Handle))]
        public async Task Should_return_result_from_call_to_DomainEventAccessor__SaveEvent()
        {
            var result = await this.Handle(this.testTransEvent, new CancellationToken());

            result.Should().BeSameAs(this.testResult);
        }
    }
}

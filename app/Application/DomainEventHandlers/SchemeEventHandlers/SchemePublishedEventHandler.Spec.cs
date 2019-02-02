using FluentAssertions;
using MediatR;
using MidnightLizard.Commons.Domain.Interfaces;
using MidnightLizard.Commons.Domain.Messaging;
using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Commons.Domain.Results;
using MidnightLizard.Impressions.Domain.PublicSchemeAggregate;
using MidnightLizard.Impressions.Domain.PublicSchemeAggregate.Events;
using MidnightLizard.Testing.Utilities;
using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;

using ITransEvent = MidnightLizard.Commons.Domain.Messaging.ITransportMessage<MidnightLizard.Commons.Domain.Messaging.BaseMessage>;
using TransEvent = MidnightLizard.Commons.Domain.Messaging.TransportMessage<MidnightLizard.Impressions.Domain.PublicSchemeAggregate.Events.SchemePublishedEvent, MidnightLizard.Impressions.Domain.PublicSchemeAggregate.PublicSchemeId>;

namespace MidnightLizard.Impressions.Processor.Application.DomainEventHandlers.SchemeEventHandlers
{
    public class SchemePublishedEventHandlerSpec : SchemePublishedEventHandler
    {
        private int handle_CallCount;
        private readonly IMediator mediator;
        private readonly ITransEvent testTransEvent = new TransEvent(new SchemePublishedEvent(null, null, null), Guid.NewGuid(), new UserId("test-user-id"), DateTime.UtcNow, DateTime.UtcNow);

        public SchemePublishedEventHandlerSpec() : base(Substitute.For<IDomainEventStore<PublicSchemeId>>())
        {
            this.mediator = StartupStub.Resolve<IMediator,
                IRequestHandler<TransportMessage<SchemePublishedEvent, PublicSchemeId>, DomainResult>>
                ((x) => this);
        }

        public override Task<DomainResult> Handle(TransEvent request, CancellationToken cancellationToken)
        {
            this.handle_CallCount++;
            return Task.FromResult(DomainResult.Ok);
        }

        [It(nameof(MediatR))]
        public async Task Should_handle_Event()
        {
            this.handle_CallCount = 0;

            var result = await this.mediator.Send(this.testTransEvent);

            this.handle_CallCount.Should().Be(1);
        }
    }
}

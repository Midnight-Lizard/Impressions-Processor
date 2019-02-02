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
    public class FailedDomainEventHandlerSpec : FailedDomainEventHandler<AccessDeniedEvent<PublicSchemeId>, PublicSchemeId>
    {
        private readonly TransportMessage<AccessDeniedEvent<PublicSchemeId>, PublicSchemeId> testTransEvent;

        public FailedDomainEventHandlerSpec()
        {
            this.testTransEvent = new TransportMessage<AccessDeniedEvent<PublicSchemeId>, PublicSchemeId>(
                null, new Guid(), new UserId("test-user-id"), DateTime.UtcNow, DateTime.UtcNow);
        }

        [It(nameof(Handle))]
        public async Task Should_always_return_Success()
        {
            var result = await this.Handle(this.testTransEvent, new CancellationToken());

            result.Should().BeSameAs(DomainResult.Ok);
        }
    }
}

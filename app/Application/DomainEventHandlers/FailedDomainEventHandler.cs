using MediatR;
using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Commons.Domain.Messaging;
using MidnightLizard.Commons.Domain.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MidnightLizard.Impressions.Processor.Application.DomainEventHandlers
{
    public abstract class FailedDomainEventHandler<TEvent, TAggregateId>
        : IRequestHandler<TransportMessage<TEvent, TAggregateId>, DomainResult>
        where TEvent : DomainEvent<TAggregateId>
        where TAggregateId : DomainEntityId
    {
        public virtual Task<DomainResult> Handle(TransportMessage<TEvent, TAggregateId> @event, CancellationToken cancellationToken)
        {
            return Task.FromResult(DomainResult.Ok);
        }
    }
}

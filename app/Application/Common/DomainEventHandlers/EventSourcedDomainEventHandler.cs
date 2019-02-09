using MediatR;
using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Commons.Domain.Interfaces;
using MidnightLizard.Commons.Domain.Messaging;
using MidnightLizard.Commons.Domain.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MidnightLizard.Impressions.Processor.Application.DomainEventHandlers
{
    public abstract class EventSourcedDomainEventHandler<TEvent, TAggregateId>
        : IRequestHandler<TransportMessage<TEvent, TAggregateId>, DomainResult>
        where TEvent : EventSourcedDomainEvent<TAggregateId>
        where TAggregateId : DomainEntityId
    {
        protected readonly IDomainEventStore<TAggregateId> domainEventAccessor;

        public EventSourcedDomainEventHandler(IDomainEventStore<TAggregateId> domainEventAccessor)
        {
            this.domainEventAccessor = domainEventAccessor;
        }

        public virtual async Task<DomainResult> Handle(TransportMessage<TEvent, TAggregateId> @event, CancellationToken cancellationToken)
        {
            var result = await this.domainEventAccessor.SaveEvent(@event);

            return result;
        }
    }
}

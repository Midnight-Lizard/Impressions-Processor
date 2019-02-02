using MidnightLizard.Commons.Domain.Interfaces;
using MidnightLizard.Impressions.Domain.PublicSchemeAggregate;
using MidnightLizard.Impressions.Domain.PublicSchemeAggregate.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Impressions.Processor.Application.DomainEventHandlers.SchemeEventHandlers
{
    public class SchemePublishedEventHandler : DomainEventHandler<SchemePublishedEvent, PublicSchemeId>
    {
        public SchemePublishedEventHandler(IDomainEventStore<PublicSchemeId> domainEventAccessor)
            : base(domainEventAccessor)
        {
        }
    }
}

using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Commons.Domain.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Impressions.Domain.PublicSchemeAggregate.Events
{
    public abstract class SchemeDomainEvent : DomainEvent<PublicSchemeId>
    {
        protected SchemeDomainEvent() : base() { }

        public SchemeDomainEvent(PublicSchemeId aggregateId) : base(aggregateId)
        {
        }
    }
}

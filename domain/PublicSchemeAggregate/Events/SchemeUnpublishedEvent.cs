using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Impressions.Domain.PublicSchemeAggregate.Events
{
    public class SchemeUnpublishedEvent : SchemeDomainEvent
    {
        protected SchemeUnpublishedEvent() { }

        public SchemeUnpublishedEvent(PublicSchemeId aggregateId) : base(aggregateId)
        {
        }
    }
}

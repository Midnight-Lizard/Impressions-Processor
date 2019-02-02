using MidnightLizard.Commons.Domain.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Impressions.Domain.PublicSchemeAggregate.Events
{
    public class SchemeAccessDeniedEvent : AccessDeniedEvent<PublicSchemeId>
    {
        protected SchemeAccessDeniedEvent() { }

        public SchemeAccessDeniedEvent(PublicSchemeId publicSchemeId) : base(publicSchemeId)
        {
        }
    }
}

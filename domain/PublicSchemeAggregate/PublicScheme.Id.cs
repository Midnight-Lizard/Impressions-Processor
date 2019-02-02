using MidnightLizard.Commons.Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Impressions.Domain.PublicSchemeAggregate
{
    public class PublicSchemeId : DomainEntityId<Guid>
    {
        public PublicSchemeId() : base()
        {
        }

        public PublicSchemeId(Guid value) : base(value)
        {
        }
    }
}

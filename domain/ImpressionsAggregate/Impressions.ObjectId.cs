using MidnightLizard.Commons.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Impressions.Domain.ImpressionsAggregate
{
    public class ImpressionsObjectId : DomainEntityId<string>
    {
        public ImpressionsObjectId() : base()
        {
        }

        public ImpressionsObjectId(string id) : base(id)
        {
        }
    }
}

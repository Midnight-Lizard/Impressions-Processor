using MidnightLizard.Commons.Domain.Messaging;
using MidnightLizard.Commons.Domain.Model;
using System.Collections.Generic;

namespace MidnightLizard.Impressions.Domain.ImpressionsAggregate.Events
{
    public abstract class ImpressionsChangedEvent<TAggregateId> : IntegrationEvent<TAggregateId>
        where TAggregateId : ImpressionsObjectId
    {
        protected IReadOnlyCollection<UserId> impressionists;

        public ImpressionsObjectType ObjectType { get; protected set; }

        protected ImpressionsChangedEvent() : base() { }

        public ImpressionsChangedEvent(
            TAggregateId aggregateId,
            ImpressionsObjectType objectType,
            IReadOnlyCollection<UserId> impressionists) : base(aggregateId)
        {
            this.ObjectType = objectType;
            this.impressionists = impressionists;
        }
    }
}

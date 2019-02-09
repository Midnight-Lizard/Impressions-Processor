using MidnightLizard.Commons.Domain.Messaging;

namespace MidnightLizard.Impressions.Domain.ImpressionsAggregate.Events
{
    public abstract class ImpressionEventSourcedDomainEvent<TAggregateId>
        : EventSourcedDomainEvent<TAggregateId>
        where TAggregateId : ImpressionsObjectId
    {
        protected ImpressionEventSourcedDomainEvent() : base() { }

        public ImpressionEventSourcedDomainEvent(TAggregateId aggregateId) : base(aggregateId)
        {
        }
    }
}

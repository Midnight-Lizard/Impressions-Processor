using MidnightLizard.Commons.Domain.Messaging;

namespace MidnightLizard.Impressions.Domain.ImpressionsAggregate.Events
{
    public abstract class ImpressionEventSourcedDomainEvent : EventSourcedDomainEvent<ImpressionsObjectId>
    {
        protected ImpressionEventSourcedDomainEvent() : base() { }

        public ImpressionEventSourcedDomainEvent(ImpressionsObjectId aggregateId) : base(aggregateId)
        {
        }
    }
}

namespace MidnightLizard.Impressions.Domain.ImpressionsAggregate.Events
{
    public abstract class ImpressionAddedEvent : ImpressionEventSourcedDomainEvent
    {
        public ImpressionsObjectType ObjectType { get; protected set; }

        protected ImpressionAddedEvent() { }

        public ImpressionAddedEvent(
            ImpressionsObjectId aggregateId,
            ImpressionsObjectType objectType) : base(aggregateId)
        {
            this.ObjectType = objectType;
        }
    }
}

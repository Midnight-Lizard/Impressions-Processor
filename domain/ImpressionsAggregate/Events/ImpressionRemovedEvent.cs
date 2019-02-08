namespace MidnightLizard.Impressions.Domain.ImpressionsAggregate.Events
{
    public abstract class ImpressionRemovedEvent : ImpressionEventSourcedDomainEvent
    {
        public ImpressionsObjectType ObjectType { get; protected set; }

        protected ImpressionRemovedEvent() { }

        public ImpressionRemovedEvent(
            ImpressionsObjectId aggregateId,
            ImpressionsObjectType objectType) : base(aggregateId)
        {
            this.ObjectType = objectType;
        }
    }
}

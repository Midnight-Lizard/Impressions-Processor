namespace MidnightLizard.Impressions.Domain.ImpressionsAggregate.Events
{
    public abstract class ImpressionAddedEvent<TAggregateId> : ImpressionEventSourcedDomainEvent<TAggregateId>
        where TAggregateId : ImpressionsObjectId
    {
        public ImpressionsObjectType ObjectType { get; protected set; }

        protected ImpressionAddedEvent() { }

        public ImpressionAddedEvent(
            TAggregateId aggregateId,
            ImpressionsObjectType objectType) : base(aggregateId)
        {
            this.ObjectType = objectType;
        }
    }
}

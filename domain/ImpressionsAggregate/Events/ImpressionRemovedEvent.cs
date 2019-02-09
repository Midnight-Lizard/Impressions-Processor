namespace MidnightLizard.Impressions.Domain.ImpressionsAggregate.Events
{
    public abstract class ImpressionRemovedEvent<TAggregateId> : ImpressionEventSourcedDomainEvent<TAggregateId>
        where TAggregateId : ImpressionsObjectId
    {
        public ImpressionsObjectType ObjectType { get; protected set; }

        protected ImpressionRemovedEvent() { }

        public ImpressionRemovedEvent(
            TAggregateId aggregateId,
            ImpressionsObjectType objectType) : base(aggregateId)
        {
            this.ObjectType = objectType;
        }
    }
}

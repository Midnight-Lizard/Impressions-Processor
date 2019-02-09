namespace MidnightLizard.Impressions.Domain.ImpressionsAggregate.Requests
{
    public class AddImpressionRequest<TAggregateId> : ImpressionsDomainRequest<TAggregateId>
        where TAggregateId : ImpressionsObjectId
    {
        public ImpressionsObjectType ObjectType { get; protected set; }
    }
}

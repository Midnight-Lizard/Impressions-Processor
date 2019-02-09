namespace MidnightLizard.Impressions.Domain.ImpressionsAggregate.Requests
{
    public class RemoveImpressionRequest<TAggregateId> : ImpressionsDomainRequest<TAggregateId>
        where TAggregateId : ImpressionsObjectId
    {
        public ImpressionsObjectType ObjectType { get; protected set; }
    }
}

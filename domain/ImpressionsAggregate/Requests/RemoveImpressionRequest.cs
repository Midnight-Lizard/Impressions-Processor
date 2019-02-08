namespace MidnightLizard.Impressions.Domain.ImpressionsAggregate.Requests
{
    public class RemoveImpressionRequest : ImpressionsDomainRequest
    {
        public ImpressionsObjectType ObjectType { get; protected set; }
    }
}

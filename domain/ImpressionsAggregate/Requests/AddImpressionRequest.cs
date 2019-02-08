namespace MidnightLizard.Impressions.Domain.ImpressionsAggregate.Requests
{
    public class AddImpressionRequest : ImpressionsDomainRequest
    {
        public ImpressionsObjectType ObjectType { get; protected set; }
    }
}

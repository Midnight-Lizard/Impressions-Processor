using MidnightLizard.Commons.Domain.Messaging;

namespace MidnightLizard.Impressions.Domain.ImpressionsAggregate.Requests
{
    public abstract class ImpressionsDomainRequest<TAggregateId> : DomainRequest<TAggregateId>
        where TAggregateId : ImpressionsObjectId
    {
    }
}

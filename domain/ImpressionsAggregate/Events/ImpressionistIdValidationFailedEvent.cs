using FluentValidation.Results;
using MidnightLizard.Commons.Domain.Messaging;

namespace MidnightLizard.Impressions.Domain.ImpressionsAggregate.Events
{
    public class ImpressionistIdValidationFailedEvent<TAggregateId> : ValidationFailedEvent<TAggregateId>
        where TAggregateId : ImpressionsObjectId
    {
        protected ImpressionistIdValidationFailedEvent() { }

        public ImpressionistIdValidationFailedEvent(
            TAggregateId aggregateId, ValidationResult validationResult)
            : base(aggregateId, validationResult)
        {
        }
    }
}

using FluentValidation.Results;
using MidnightLizard.Commons.Domain.Messaging;

namespace MidnightLizard.Impressions.Domain.ImpressionsAggregate.Events
{
    public class ImpressionistIdValidationFailedEvent : ValidationFailedEvent<ImpressionsObjectId>
    {
        protected ImpressionistIdValidationFailedEvent() { }

        public ImpressionistIdValidationFailedEvent(
            ImpressionsObjectId aggregateId, ValidationResult validationResult)
            : base(aggregateId, validationResult)
        {
        }
    }
}

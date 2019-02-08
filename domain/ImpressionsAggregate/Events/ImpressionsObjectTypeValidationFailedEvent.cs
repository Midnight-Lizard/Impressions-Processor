using FluentValidation.Results;
using MidnightLizard.Commons.Domain.Messaging;

namespace MidnightLizard.Impressions.Domain.ImpressionsAggregate.Events
{
    public class ImpressionsObjectTypeValidationFailedEvent : ValidationFailedEvent<ImpressionsObjectId>
    {
        protected ImpressionsObjectTypeValidationFailedEvent() { }

        public ImpressionsObjectTypeValidationFailedEvent(
            ImpressionsObjectId aggregateId, ValidationResult validationResult)
            : base(aggregateId, validationResult)
        {
        }
    }
}

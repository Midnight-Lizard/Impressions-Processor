using FluentValidation.Results;
using MidnightLizard.Commons.Domain.Messaging;

namespace MidnightLizard.Impressions.Domain.ImpressionsAggregate.Events
{
    public class ImpressionsObjectTypeValidationFailedEvent<TAggregateId> : ValidationFailedEvent<TAggregateId>
        where TAggregateId : ImpressionsObjectId
    {
        protected ImpressionsObjectTypeValidationFailedEvent() { }

        public ImpressionsObjectTypeValidationFailedEvent(
            TAggregateId aggregateId, ValidationResult validationResult)
            : base(aggregateId, validationResult)
        {
        }
    }
}

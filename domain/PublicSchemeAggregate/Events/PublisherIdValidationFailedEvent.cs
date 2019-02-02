using FluentValidation.Results;
using MidnightLizard.Commons.Domain.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Impressions.Domain.PublicSchemeAggregate.Events
{
    public class PublisherIdValidationFailedEvent : ValidationFailedEvent<PublicSchemeId>
    {
        protected PublisherIdValidationFailedEvent() { }

        public PublisherIdValidationFailedEvent(PublicSchemeId aggregateId, ValidationResult validationResult)
            : base(aggregateId, validationResult)
        {
        }
    }
}

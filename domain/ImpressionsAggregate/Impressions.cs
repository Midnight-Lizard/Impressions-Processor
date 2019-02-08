using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Impressions.Domain.ImpressionsAggregate.Events;
using System.Collections.Generic;
using System.Linq;

namespace MidnightLizard.Impressions.Domain.ImpressionsAggregate
{
    public abstract partial class Impressions : AggregateRoot<ImpressionsObjectId>
    {
        protected HashSet<UserId> _Impressionists = new HashSet<UserId>();

        public ImpressionsObjectType ObjectType { get; protected set; }

        public Impressions() { }
        public Impressions(ImpressionsObjectId objectId) : base(objectId) { }

        protected abstract ImpressionAddedEvent CreateImpressionAddedEvent(
            ImpressionsObjectType objectType);

        protected abstract ImpressionRemovedEvent CreateImpressionRemovedEvent(
            ImpressionsObjectType objectType);

        protected abstract ImpressionsChangedEvent CreateImpressionsChangedEvent();

        private bool ImpressionistIsValid(UserId impressionistId)
        {
            var validationResults = new DomainEntityIdValidator<string>().Validate(impressionistId);
            if (!validationResults.IsValid)
            {
                this.AddFailedDomainEvent(new ImpressionistIdValidationFailedEvent(this.Id, validationResults));
                return false;
            }

            return true;
        }

        private bool ObjectTypeIsValid(ImpressionsObjectType objectType)
        {
            var validationResult = ImpressionsObjectType.Validator.Validate(objectType);
            if (!validationResult.IsValid)
            {
                this.AddFailedDomainEvent(new ImpressionsObjectTypeValidationFailedEvent(this.Id, validationResult));
                return false;
            }
            return true;
        }

        public virtual void AddImpression(UserId impressionistId, ImpressionsObjectType objectType)
        {
            if (this.ImpressionistIsValid(impressionistId) &&
                this.ObjectTypeIsValid(objectType) &&
                !this._Impressionists.Contains(impressionistId))
            {
                this.AddEventSourcedDomainEvent(this.CreateImpressionAddedEvent(objectType), impressionistId);
                this.AddIntegrationEvent(this.CreateImpressionsChangedEvent());
            }
        }

        public virtual void RemoveImpression(UserId impressionistId, ImpressionsObjectType objectType)
        {
            if (this.ImpressionistIsValid(impressionistId) &&
                this.ObjectTypeIsValid(objectType) &&
                this._Impressionists.Contains(impressionistId))
            {
                this.AddEventSourcedDomainEvent(this.CreateImpressionRemovedEvent(objectType), impressionistId);
                this.AddIntegrationEvent(this.CreateImpressionsChangedEvent());
            }
        }
    }
}

using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Impressions.Domain.ImpressionsAggregate.Events;
using System.Collections.Generic;

namespace MidnightLizard.Impressions.Domain.ImpressionsAggregate
{
    public abstract partial class Impressions<TAggregateId> : AggregateRoot<TAggregateId>
        where TAggregateId : ImpressionsObjectId
    {
        protected HashSet<UserId> _Impressionists = new HashSet<UserId>();

        public ImpressionsObjectType ObjectType { get; protected set; }

        public Impressions() { }
        public Impressions(TAggregateId objectId) : base(objectId) { }

        protected abstract ImpressionAddedEvent<TAggregateId> CreateImpressionAddedEvent(
            ImpressionsObjectType objectType);

        protected abstract ImpressionRemovedEvent<TAggregateId> CreateImpressionRemovedEvent(
            ImpressionsObjectType objectType);

        protected abstract ImpressionsChangedEvent<TAggregateId> CreateImpressionsChangedEvent();

        private bool ImpressionistIsValid(UserId impressionistId)
        {
            var validationResults = new DomainEntityIdValidator<string>().Validate(impressionistId);
            if (!validationResults.IsValid)
            {
                this.AddFailedDomainEvent(new ImpressionistIdValidationFailedEvent<TAggregateId>(this.Id, validationResults));
                return false;
            }

            return true;
        }

        private bool ObjectTypeIsValid(ImpressionsObjectType objectType)
        {
            var validationResult = ImpressionsObjectType.Validator.Validate(objectType);
            if (!validationResult.IsValid)
            {
                this.AddFailedDomainEvent(new ImpressionsObjectTypeValidationFailedEvent<TAggregateId>(this.Id, validationResult));
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

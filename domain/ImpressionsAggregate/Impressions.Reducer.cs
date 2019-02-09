using MidnightLizard.Commons.Domain.Messaging;
using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Impressions.Domain.ImpressionsAggregate.Events;

namespace MidnightLizard.Impressions.Domain.ImpressionsAggregate
{
    public abstract partial class Impressions<TAggregateId> : AggregateRoot<TAggregateId>
        where TAggregateId : ImpressionsObjectId
    {
        protected override void Reduce(EventSourcedDomainEvent<TAggregateId> @event, UserId impressionistId)
        {
            switch (@event)
            {
                case ImpressionAddedEvent<TAggregateId> addedEvent:
                    this.ObjectType = addedEvent.ObjectType;
                    this._Impressionists.Add(impressionistId);
                    break;

                case ImpressionRemovedEvent<TAggregateId> removedEvent:
                    this.ObjectType = removedEvent.ObjectType;
                    this._Impressionists.Remove(impressionistId);
                    break;

                default:
                    break;
            }
        }
    }
}

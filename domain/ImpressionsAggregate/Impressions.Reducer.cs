using MidnightLizard.Commons.Domain.Messaging;
using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Impressions.Domain.ImpressionsAggregate.Events;

namespace MidnightLizard.Impressions.Domain.ImpressionsAggregate
{
    public abstract partial class Impressions : AggregateRoot<ImpressionsObjectId>
    {
        protected override void Reduce(EventSourcedDomainEvent<ImpressionsObjectId> @event, UserId impressionistId)
        {
            switch (@event)
            {
                case ImpressionAddedEvent addedEvent:
                    this.ObjectType = addedEvent.ObjectType;
                    this._Impressionists.Add(impressionistId);
                    break;

                case ImpressionRemovedEvent removedEvent:
                    this.ObjectType = removedEvent.ObjectType;
                    this._Impressionists.Remove(impressionistId);
                    break;

                default:
                    break;
            }
        }
    }
}

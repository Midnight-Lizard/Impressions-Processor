using MidnightLizard.Commons.Domain.Messaging;
using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Impressions.Domain.PublicSchemeAggregate.Events;

namespace MidnightLizard.Impressions.Domain.PublicSchemeAggregate
{
    public partial class PublicScheme : AggregateRoot<PublicSchemeId>
    {
        public override void Reduce(DomainEvent<PublicSchemeId> @event, UserId publisherId)
        {
            switch (@event)
            {
                case SchemePublishedEvent published:
                    this.PublisherId = publisherId;
                    this.ColorScheme = published.ColorScheme;
                    this.Description = published.Description;
                    break;

                default:
                    break;
            }
        }
    }
}

using FluentValidation.Results;
using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Impressions.Domain.PublicSchemeAggregate.Events;

namespace MidnightLizard.Impressions.Domain.PublicSchemeAggregate
{

    public partial class PublicScheme : AggregateRoot<PublicSchemeId>
    {
        private const int maxDescriptionLength = 2000;
        public UserId PublisherId { get; private set; }
        public string Description { get; private set; }
        public ColorScheme ColorScheme { get; private set; }

        public PublicScheme() { }
        public PublicScheme(PublicSchemeId publicSchemeId) : base(publicSchemeId) { }

        public virtual void Publish(UserId publisherId, ColorScheme colorScheme, string description)
        {
            if (this.PublisherIsValid(publisherId) &&
                this.DescriptionIsValid(publisherId, description) &&
                this.ColorSchemeIsValid(publisherId, colorScheme))
            {
                if (this.IsNew() ||
                    description != this.Description ||
                    colorScheme != this.ColorScheme)
                {
                    this.AddSchemePublishedEvent(publisherId, colorScheme, description);
                }
            }
        }

        public virtual void Unpublish(UserId publisherId)
        {
            if (this.PublisherIsValid(publisherId))
            {
                if (this.IsNew())
                {
                    this.AddSchemeNotFoundEvent(publisherId);
                }
                else
                {
                    this.AddSchemeUnpublishedEvent(publisherId);
                }
            }
        }

        private bool PublisherIsValid(UserId publisherId)
        {
            var publisherIdValidationResults = new DomainEntityIdValidator<string>().Validate(publisherId);
            if (!publisherIdValidationResults.IsValid)
            {
                this.AddDomainEvent(new PublisherIdValidationFailedEvent(this.Id, publisherIdValidationResults), publisherId);
                return false;
            }

            if (!this.IsNew() && this.PublisherId != publisherId)
            {
                this.AddDomainEvent(new SchemeAccessDeniedEvent(this.Id), publisherId);
                return false;
            }

            return true;
        }

        private bool ColorSchemeIsValid(UserId publisherId, ColorScheme colorScheme)
        {
            var colorSchemeValidationResults = ColorScheme.Validator.Validate(colorScheme);
            if (!colorSchemeValidationResults.IsValid)
            {
                this.AddDomainEvent(new ColorSchemeValidationFailedEvent(this.Id, colorSchemeValidationResults),
                    publisherId);
                return false;
            }
            return true;
        }

        private bool DescriptionIsValid(UserId publisherId, string description)
        {
            if (description != null && description.Length > maxDescriptionLength)
            {
                var error = new ValidationResult(new[] {
                    new ValidationFailure(nameof(PublicScheme.Description),
                    $"Public scheme description should not be longer than {maxDescriptionLength} symbols.")
                });
                this.AddDomainEvent(new PublicSchemeDescriptionValidationFailedEvent(this.Id, error), publisherId);
                return false;
            }
            return true;
        }

        private void AddSchemeNotFoundEvent(UserId publisherId)
        {
            // ignoring - since there is no such scheme - no need to delete it
        }

        private void AddSchemeUnpublishedEvent(UserId publisherId)
        {
            this.AddDomainEvent(new SchemeUnpublishedEvent(this.Id), publisherId);
        }

        private void AddSchemePublishedEvent(UserId publisherId, ColorScheme colorScheme, string description)
        {
            this.AddDomainEvent(new SchemePublishedEvent(this.Id, colorScheme, description), publisherId);
        }
    }
}

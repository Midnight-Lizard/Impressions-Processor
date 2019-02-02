using FluentAssertions;
using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Impressions.Domain.PublicSchemeAggregate.Events;
using MidnightLizard.Testing.Utilities;
using System;

namespace MidnightLizard.Impressions.Domain.PublicSchemeAggregate
{
    public class PublicSchemeSpec
    {
        private readonly string description = "test description";
        private readonly PublicScheme newPublicScheme;
        private readonly PublicScheme existingPublicScheme;
        private readonly UserId testPublisherId = new UserId("test-user-id");
        private readonly PublicSchemeId testPublicSchemeId = new PublicSchemeId(Guid.NewGuid());

        public PublicSchemeSpec()
        {
            this.existingPublicScheme = new PublicScheme(this.testPublicSchemeId);
            this.existingPublicScheme.Publish(this.testPublisherId, ColorSchemeSpec.CorrectColorScheme, this.description);
            this.existingPublicScheme.ReleaseEvents();

            this.newPublicScheme = new PublicScheme(this.testPublicSchemeId);
        }

        public class UnpublishSpec : PublicSchemeSpec
        {
            [It(nameof(PublicScheme.Unpublish))]
            public void Should_not_release_any_new_events_if_PublicScheme__IsNew()
            {
                this.newPublicScheme.Unpublish(this.testPublisherId);
                var events = this.newPublicScheme.ReleaseEvents();
                events.Should().BeEmpty();
            }

            [It(nameof(PublicScheme.Publish))]
            public void Should_Release_AccessDenied_when_Publisher_is_different()
            {
                this.existingPublicScheme.Unpublish(new UserId("different-user-id"));
                var events = this.existingPublicScheme.ReleaseEvents();
                events.Should().ContainSingle();
                events.Should().AllBeOfType<SchemeAccessDeniedEvent>();
            }

            [It(nameof(PublicScheme.Publish))]
            public void Should_Release_ValidationFailedEvent_when_PublisherId_is_invalid()
            {
                this.existingPublicScheme.Unpublish(new UserId(null));
                var events = this.existingPublicScheme.ReleaseEvents();
                events.Should().ContainSingle();
                events.Should().AllBeOfType<PublisherIdValidationFailedEvent>();
            }

            [It(nameof(PublicScheme.Publish))]
            public void Should_Release_SchemeUnpublishedEvent_when_everything_is_ok()
            {
                this.existingPublicScheme.Unpublish(this.testPublisherId);
                var events = this.existingPublicScheme.ReleaseEvents();
                events.Should().ContainSingle();
                events.Should().AllBeOfType<SchemeUnpublishedEvent>();
            }
        }

        public class PiblishSpec : PublicSchemeSpec
        {
            private readonly ColorScheme incorrectColorScheme = new ColorScheme();

            [It(nameof(PublicScheme.Publish))]
            public void Should_Release_SchemePublishedEvent_when_everything_is_ok()
            {
                var newColorScheme = ColorSchemeSpec.CorrectColorScheme;
                newColorScheme.colorSchemeName = "new name";
                this.existingPublicScheme.Publish(this.testPublisherId, newColorScheme, this.description);
                var events = this.existingPublicScheme.ReleaseEvents();
                events.Should().ContainSingle();
                events.Should().AllBeOfType<SchemePublishedEvent>();
            }

            [It(nameof(PublicScheme.Publish))]
            public void Should_not_be_New_after_successful_Publish()
            {
                this.newPublicScheme.Publish(this.testPublisherId, ColorSchemeSpec.CorrectColorScheme, this.description);
                this.newPublicScheme.IsNew().Should().BeFalse();
            }

            [It(nameof(PublicScheme.Publish))]
            public void Should_not_Release_any_new_Events_if_ColorScheme_and_Description_are_the_same_as_previously()
            {
                this.existingPublicScheme.Publish(this.testPublisherId, ColorSchemeSpec.CorrectColorScheme, this.description);
                var events = this.existingPublicScheme.ReleaseEvents();
                events.Should().BeEmpty();
            }

            [It(nameof(PublicScheme.Publish))]
            public void Should_not_Release_ValidationError_Event_if_Description_is_too_long()
            {
                this.existingPublicScheme.Publish(this.testPublisherId, ColorSchemeSpec.CorrectColorScheme,
                    new string('-', 2001));
                var events = this.existingPublicScheme.ReleaseEvents();
                events.Should().ContainSingle();
                events.Should().AllBeOfType<PublicSchemeDescriptionValidationFailedEvent>();
            }

            [It(nameof(PublicScheme.Publish))]
            public void Should_Release_ValidationFailedEvent_when_ColorScheme_is_invalid()
            {
                this.existingPublicScheme.Publish(this.testPublisherId, this.incorrectColorScheme, this.description);
                var events = this.existingPublicScheme.ReleaseEvents();
                events.Should().ContainSingle();
                events.Should().AllBeOfType<ColorSchemeValidationFailedEvent>();
            }

            [It(nameof(PublicScheme.Publish))]
            public void Should_Release_AccessDenied_when_Publisher_is_different()
            {
                this.existingPublicScheme.Publish(new UserId("different-user-id"), this.incorrectColorScheme, this.description);
                var events = this.existingPublicScheme.ReleaseEvents();
                events.Should().ContainSingle();
                events.Should().AllBeOfType<SchemeAccessDeniedEvent>();
            }

            [It(nameof(PublicScheme.Publish))]
            public void Should_Release_ValidationFailedEvent_when_PublisherId_is_invalid()
            {
                this.existingPublicScheme.Publish(new UserId(null), this.incorrectColorScheme, this.description);
                var events = this.existingPublicScheme.ReleaseEvents();
                events.Should().ContainSingle();
                events.Should().AllBeOfType<PublisherIdValidationFailedEvent>();
            }
        }
    }
}

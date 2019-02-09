using FluentAssertions;
using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Impressions.Domain.ImpressionsAggregate.Events;
using MidnightLizard.Testing.Utilities;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;

namespace MidnightLizard.Impressions.Domain.ImpressionsAggregate
{
    public class ImpressionsSpec : Impressions<ImpressionsObjectId>
    {
        private readonly UserId testImpressionist = new UserId("test-user-id");
        private readonly ImpressionsObjectType testObjectType = new ImpressionsObjectType("test");
        private readonly ImpressionsObjectId testObjectId = new ImpressionsObjectId("test-obj-id");
        private readonly ImpressionsSpec newImpressions;
        private readonly ImpressionsSpec existingImpressions;

        public ImpressionsSpec(ImpressionsObjectId id) : base(id) { }
        public ImpressionsSpec()
        {
            this.newImpressions = Substitute.ForPartsOf<ImpressionsSpec>(this.testObjectId);
            this.existingImpressions = Substitute.ForPartsOf<ImpressionsSpec>(this.testObjectId);
            this.existingImpressions.AddImpression(this.testImpressionist, this.testObjectType);
            this.existingImpressions.ReleaseEvents();
        }

        protected override ImpressionAddedEvent<ImpressionsObjectId> CreateImpressionAddedEvent(ImpressionsObjectType objectType)
        {
            return Substitute.ForPartsOf<ImpressionAddedEvent<ImpressionsObjectId>>(this.Id, objectType);
        }

        protected override ImpressionsChangedEvent<ImpressionsObjectId> CreateImpressionsChangedEvent()
        {
            return Substitute.ForPartsOf<ImpressionsChangedEvent<ImpressionsObjectId>>(this.Id, this.ObjectType, this._Impressionists);
        }

        protected override ImpressionRemovedEvent<ImpressionsObjectId> CreateImpressionRemovedEvent(ImpressionsObjectType objectType)
        {
            return Substitute.ForPartsOf<ImpressionRemovedEvent<ImpressionsObjectId>>(this.Id, objectType);
        }

        public class AddImpressionSpec : ImpressionsSpec
        {
            [It(nameof(Impressions<ImpressionsObjectId>.AddImpression))]
            public void Should_Release_correct_events_when_AddImpression_is_called()
            {
                this.newImpressions.AddImpression(this.testImpressionist, this.testObjectType);
                var events = this.newImpressions.ReleaseEvents();
                events.Should().HaveCount(2);
                events.First().Should().BeAssignableTo<ImpressionAddedEvent<ImpressionsObjectId>>();
                events.Last().Should().BeAssignableTo<ImpressionsChangedEvent<ImpressionsObjectId>>();
            }

            [It(nameof(Impressions<ImpressionsObjectId>.AddImpression))]
            public void Should_set_ObjectType_when_AddImpression_is_called_first_time()
            {
                this.newImpressions.AddImpression(this.testImpressionist, this.testObjectType);
                this.newImpressions.ObjectType.Should().Be(this.testObjectType);
            }

            [It(nameof(Impressions<ImpressionsObjectId>.AddImpression))]
            public void Should_change_ObjectType_when_AddImpression_is_called_second_time()
            {
                this.existingImpressions.ObjectType.Should().Be(this.testObjectType);

                var newObjectType = new ImpressionsObjectType("new-type");
                this.existingImpressions.AddImpression(new UserId("other-one"), newObjectType);

                this.existingImpressions.ObjectType.Should().Be(newObjectType);
            }

            [It(nameof(Impressions<ImpressionsObjectId>.AddImpression))]
            public void Should_add_user_to_Impressionists_list_when_AddImpression_is_called()
            {
                var testImpressionists = new List<UserId>
                {
                    new UserId("1"),
                    new UserId("2"),
                    new UserId("3"),
                    new UserId("4"),
                };
                var done = new List<UserId>();

                foreach (var user in testImpressionists)
                {
                    done.Add(user);
                    this.newImpressions.AddImpression(user, this.testObjectType);
                    this.newImpressions._Impressionists.Should().BeEquivalentTo(done);
                }

                this.newImpressions.ReleaseEvents().Should().HaveCount(testImpressionists.Count * 2);
            }

            [It(nameof(Impressions<ImpressionsObjectId>.AddImpression))]
            public void Should_not_add_Impressionist_to_Impressionists_list_twice_when_AddImpression_is_called()
            {
                this.existingImpressions.AddImpression(this.testImpressionist, this.testObjectType);
                this.existingImpressions._Impressionists.Should().ContainSingle();
                this.existingImpressions._Impressionists.Should().Contain(this.testImpressionist);
            }

            [It(nameof(Impressions<ImpressionsObjectId>.AddImpression))]
            public void Should_not_Release_events_twice_when_AddImpression_is_called_by_the_same_user()
            {
                this.existingImpressions.AddImpression(this.testImpressionist, this.testObjectType);
                var events = this.newImpressions.ReleaseEvents();
                events.Should().BeEmpty();
            }
        }

        public class RemoveImpressionSpec : ImpressionsSpec
        {
            [It(nameof(Impressions<ImpressionsObjectId>.RemoveImpression))]
            public void Should_Release_correct_events_when_RemoveImpression_is_called()
            {
                this.existingImpressions.RemoveImpression(this.testImpressionist, this.testObjectType);
                var events = this.existingImpressions.ReleaseEvents();
                events.Should().HaveCount(2);
                events.First().Should().BeAssignableTo<ImpressionRemovedEvent<ImpressionsObjectId>>();
                events.Last().Should().BeAssignableTo<ImpressionsChangedEvent<ImpressionsObjectId>>();
            }

            [It(nameof(Impressions<ImpressionsObjectId>.RemoveImpression))]
            public void Should_not_set_ObjectType_when_RemoveImpression_is_called_first_time()
            {
                this.newImpressions.RemoveImpression(this.testImpressionist, this.testObjectType);
                this.newImpressions.ObjectType.Should().BeNull();
            }

            [It(nameof(Impressions<ImpressionsObjectId>.RemoveImpression))]
            public void Should_not_Release_events_when_RemoveImpression_is_called_without_AddImpression()
            {
                this.newImpressions.RemoveImpression(this.testImpressionist, this.testObjectType);
                var events = this.newImpressions.ReleaseEvents();
                events.Should().BeEmpty();
            }

            [It(nameof(Impressions<ImpressionsObjectId>.RemoveImpression))]
            public void Should_change_ObjectType_when_RemoveImpression_is_called_second_time()
            {
                this.existingImpressions.ObjectType.Should().Be(this.testObjectType);

                var newObjectType = new ImpressionsObjectType("new-type");
                this.existingImpressions.RemoveImpression(this.testImpressionist, newObjectType);

                this.existingImpressions.ObjectType.Should().Be(newObjectType);
            }

            [It(nameof(Impressions<ImpressionsObjectId>.RemoveImpression))]
            public void Should_remove_user_from_Impressionists_list_when_RemoveImpression_is_called()
            {
                var testImpressionists = new List<UserId>
                {
                    new UserId("1"),
                    new UserId("2"),
                    new UserId("3"),
                    new UserId("4"),
                };
                var done = new List<UserId>();

                foreach (var user in testImpressionists)
                {
                    done.Add(user);
                    this.newImpressions.AddImpression(user, this.testObjectType);
                }

                foreach (var user in testImpressionists)
                {
                    done.Remove(user);
                    this.newImpressions.RemoveImpression(user, this.testObjectType);
                    this.newImpressions._Impressionists.Should().BeEquivalentTo(done);
                }

                this.newImpressions.ReleaseEvents().Should().HaveCount(testImpressionists.Count * 4);
            }

            [It(nameof(Impressions<ImpressionsObjectId>.RemoveImpression))]
            public void Should_not_Release_events_twice_when_RemoveImpression_is_called_by_the_same_user()
            {
                this.existingImpressions.RemoveImpression(this.testImpressionist, this.testObjectType);
                this.existingImpressions.ReleaseEvents();

                this.existingImpressions.RemoveImpression(this.testImpressionist, this.testObjectType);

                var events = this.existingImpressions.ReleaseEvents();
                events.Should().BeEmpty();
            }
        }
    }
}

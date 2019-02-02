using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MidnightLizard.Commons.Domain.Interfaces;
using MidnightLizard.Commons.Domain.Messaging;
using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Commons.Domain.Results;
using MidnightLizard.Impressions.Domain.PublicSchemeAggregate;
using MidnightLizard.Impressions.Domain.PublicSchemeAggregate.Requests;
using MidnightLizard.Impressions.Processor.Configuration;
using MidnightLizard.Testing.Utilities;
using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;

using ITransRequest = MidnightLizard.Commons.Domain.Messaging.ITransportMessage<MidnightLizard.Commons.Domain.Messaging.BaseMessage>;
using TransRequest = MidnightLizard.Commons.Domain.Messaging.TransportMessage<MidnightLizard.Impressions.Domain.PublicSchemeAggregate.Requests.PublishSchemeRequest, MidnightLizard.Impressions.Domain.PublicSchemeAggregate.PublicSchemeId>;

namespace MidnightLizard.Impressions.Processor.Application.DomainRequestHandlers
{
    public class SchemePublishRequestHandlerSpec : SchemePublishRequestHandler
    {
        private readonly string description = "test description";
        private readonly PublicScheme testScheme = Substitute.For<PublicScheme>();
        private readonly PublishSchemeRequest testRequest = Substitute.For<PublishSchemeRequest>();
        private readonly UserId testUserId = new UserId("test-user-id");

        protected SchemePublishRequestHandlerSpec() : base(
            Substitute.For<IOptions<AggregatesConfig>>(),
            Substitute.For<IMemoryCache>(),
            Substitute.For<IDomainEventDispatcher<PublicSchemeId>>(),
            Substitute.For<IAggregateSnapshotAccessor<PublicScheme, PublicSchemeId>>(),
            Substitute.For<IDomainEventStore<PublicSchemeId>>())
        {
            this.testScheme.Id.Returns(new PublicSchemeId());
            this.testRequest.AggregateId.Returns(this.testScheme.Id);
            this.testRequest.Description.Returns(this.description);
            this.testRequest.ColorScheme.Returns(ColorSchemeSpec.CorrectColorScheme);
        }

        public class HandleDomainRequestSpec : SchemePublishRequestHandlerSpec
        {
            public HandleDomainRequestSpec() : base()
            {
            }

            [It(nameof(HandleDomainRequest))]
            public void Should_call_PublicScheme__Publish_with_corresponding_parameters()
            {
                this.HandleDomainRequest(this.testScheme, this.testRequest, this.testUserId, new CancellationToken());

                this.testScheme.Received(1).Publish(this.testUserId, ColorSchemeSpec.CorrectColorScheme, this.description);
            }
        }

        public class MediatorSpec : SchemePublishRequestHandlerSpec
        {
            private readonly IMediator mediator;
            private readonly ITransRequest testTransRequest;
            private int handle_CallCount;

            public MediatorSpec()
            {
                this.testTransRequest = new TransRequest(new PublishSchemeRequest(), Guid.NewGuid(), this.testUserId, DateTime.UtcNow, DateTime.UtcNow);

                this.mediator = StartupStub.Resolve<IMediator,
                    IRequestHandler<TransportMessage<PublishSchemeRequest, PublicSchemeId>, DomainResult>>
                    ((x) => this);
            }

            public override Task<DomainResult> Handle(TransRequest transRequest, CancellationToken cancellationToken)
            {
                this.handle_CallCount++;
                return Task.FromResult(DomainResult.Ok);
            }

            [It(nameof(MediatR))]
            public async Task Should_handle_Request()
            {
                this.handle_CallCount = 0;

                var result = await this.mediator.Send(this.testTransRequest);

                this.handle_CallCount.Should().Be(1);
            }
        }
    }
}

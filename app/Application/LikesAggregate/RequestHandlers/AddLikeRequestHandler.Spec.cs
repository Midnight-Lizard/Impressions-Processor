using FluentAssertions;
using MediatR;
using MidnightLizard.Commons.Domain.Messaging;
using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Commons.Domain.Results;
using MidnightLizard.Impressions.Domain.LikesAggregate;
using MidnightLizard.Impressions.Domain.LikesAggregate.Requests;
using MidnightLizard.Testing.Utilities;
using System;
using System.Threading;
using System.Threading.Tasks;
using TransRequest = MidnightLizard.Commons.Domain.Messaging.TransportMessage<MidnightLizard.Impressions.Domain.LikesAggregate.Requests.AddLikeRequest, MidnightLizard.Impressions.Domain.LikesAggregate.LikesId>;


namespace MidnightLizard.Impressions.Processor.Application.LikesAggregate.RequestHandlers
{
    public class AddLikeRequestHandlerSpec : AddLikeRequestHandler
    {
        private readonly IMediator mediator;
        private readonly TransRequest testTransRequest;
        private readonly UserId testUserId = new UserId("test-user-id");
        private int handle_CallCount;

        public AddLikeRequestHandlerSpec() : base(null, null, null, null, null)
        {
            this.testTransRequest = new TransRequest(new AddLikeRequest(), Guid.NewGuid(), this.testUserId, DateTime.UtcNow, DateTime.UtcNow);

            this.mediator = StartupStub.Resolve<IMediator,
                IRequestHandler<TransportMessage<AddLikeRequest, LikesId>, DomainResult>>
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

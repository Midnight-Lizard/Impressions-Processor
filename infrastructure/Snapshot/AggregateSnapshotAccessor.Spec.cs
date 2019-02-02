using Elasticsearch.Net;
using FluentAssertions;
using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Impressions.Infrastructure.Versioning;
using MidnightLizard.Impressions.Domain.PublicSchemeAggregate;
using MidnightLizard.Impressions.Infrastructure.Configuration;
using MidnightLizard.Impressions.Infrastructure.Serialization.Common;
using MidnightLizard.Testing.Utilities;
using Nest;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidnightLizard.Impressions.Infrastructure.Snapshot
{
    public class AggregateSnapshotAccessorSpec : AggregateSnapshotAccessor<PublicScheme, PublicSchemeId>
    {
        protected int ApplyAggregateMappingsOnIndex_CallCount = 0;
        protected int CreateNewAggregate_CallCount = 0;
        protected string requestUri;

        protected override string IndexName => "test";

        public AggregateSnapshotAccessorSpec() : base(
            Substitute.For<SchemaVersion>(SchemaVersion.Latest.ToString()),
            Substitute.For<ElasticSearchConfig>())
        {
        }

        protected override IPromise<IMappings> ApplyAggregateMappingsOnIndex(MappingsDescriptor md)
        {
            ApplyAggregateMappingsOnIndex_CallCount++;
            return md;
        }

        protected override PublicScheme CreateNewAggregate(PublicSchemeId id)
        {
            CreateNewAggregate_CallCount++;
            return new PublicScheme(id);
        }

        public override Task Save(AggregateSnapshot<PublicScheme, PublicSchemeId> aggregate)
        {
            return Task.CompletedTask;
        }

        protected override IElasticClient CreateElasticClient()
        {
            return new ElasticClient(ApplyAggregateMappingsOnConnection(new ConnectionSettings(
                new SingleNodeConnectionPool(new Uri("http://test.com")), new InMemoryConnection(),
                (builtin, settings) => new AggregateSerializer()).EnableDebugMode(OnRequestCompleted)));

        }

        protected virtual void OnRequestCompleted(IApiCallDetails x)
        {
            if (x.RequestBodyInBytes != null && x.RequestBodyInBytes.Length > 1)
            {
                var body = Encoding.UTF8.GetString(x.RequestBodyInBytes);
            }
            this.requestUri = x.Uri.ToString();
        }

        public class CreateIndexSpec : AggregateSnapshotAccessorSpec
        {
            [It(nameof(CreateIndex))]
            public void Should_call_ApplyAggregateMappingsOnIndex()
            {
                this.CreateIndex();

                this.ApplyAggregateMappingsOnIndex_CallCount.Should().Be(1);
            }

            [It(nameof(CreateIndex))]
            public void Should_use_ElasticSearchConfig()
            {
                this.CreateIndex();

                var shards = this.config.Received(1).ELASTIC_SEARCH_SNAPSHOT_SHARDS;
                var replicas = this.config.Received(1).ELASTIC_SEARCH_SNAPSHOT_REPLICAS;
            }
        }

        public class ReadSpec : AggregateSnapshotAccessorSpec
        {
            private IGetResponse<PublicScheme> testResponse;
            private readonly PublicSchemeId testSchemeId = new PublicSchemeId(Guid.NewGuid());
            private readonly PublicScheme testScheme;

            public ReadSpec()
            {
                testScheme = Substitute.For<PublicScheme>(testSchemeId);

                this.testResponse.Source
                    .Returns(testScheme);

                this.testResponse.Fields
                    .Returns(Substitute.For<FieldValues>());
            }

            protected override IElasticClient CreateElasticClient()
            {
                this.testResponse = Substitute.For<IGetResponse<PublicScheme>>();
                var realElastic = base.CreateElasticClient();
                var stubElastic = Substitute.For<IElasticClient>();
                stubElastic.GetAsync<PublicScheme>(Arg.Any<DocumentPath<PublicScheme>>())
                    .Returns(x =>
                    {
                        realElastic.Get<PublicScheme>(x.Arg<DocumentPath<PublicScheme>>());
                        return this.testResponse;
                    });
                return stubElastic;
            }

            [It(nameof(Read))]
            public async Task Should_send_request_to_correct_Uri()
            {
                var result = await this.Read(testSchemeId);

                this.requestUri.Should().Contain($"{this.IndexName}/snapshot/{testSchemeId}");
            }

            [It(nameof(Read))]
            public async Task Should_call_ElasticClient__GetAsync()
            {
                var result = await this.Read(testSchemeId);

                await this.elasticClient.Received(1).GetAsync<PublicScheme>(Arg.Any<DocumentPath<PublicScheme>>());
            }

            [It(nameof(Read))]
            public async Task Should_return_Aggregate_from_response_if_it_IsValid()
            {
                this.testResponse.IsValid.Returns(true);
                this.version.ToString().Returns(null as string);

                var result = await this.Read(testSchemeId);

                result.Aggregate.Should().BeSameAs(this.testScheme);
            }

            [It(nameof(Read))]
            public async Task Should_return_a_new_Aggregate_from_response_if_it_IsValid_but_has_different_Version()
            {
                this.testResponse.IsValid.Returns(true);
                this.version.Value.Returns(new SemVer.Version("1.2.3-beta"));

                var result = await this.Read(testSchemeId);

                result.Aggregate.Should().NotBeSameAs(this.testScheme);
            }

            [It(nameof(Read))]
            public async Task Should_call_CreateNewAggregate_if_response__IsValid_but_has_different_Version()
            {
                this.testResponse.IsValid.Returns(true);
                this.version.Value.Returns(new SemVer.Version("1.2.3-beta"));

                var result = await this.Read(testSchemeId);

                this.CreateNewAggregate_CallCount.Should().Be(1);
            }
        }
    }
}

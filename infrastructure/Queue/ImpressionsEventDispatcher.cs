using MidnightLizard.Impressions.Domain.ImpressionsAggregate;
using MidnightLizard.Impressions.Infrastructure.Configuration;
using MidnightLizard.Impressions.Infrastructure.Serialization.Common;

namespace MidnightLizard.Impressions.Infrastructure.Queue
{
    public class ImpressionsEventDispatcher : DomainEventDispatcher<ImpressionsObjectId>
    {
        private readonly ImpressionsKafkaConfig impressionsKafkaConfig;

        public ImpressionsEventDispatcher(
            ImpressionsKafkaConfig impressionsKafkaConfig,
            KafkaConfig kafkaConfig,
            IMessageSerializer messageSerializer) : base(kafkaConfig, messageSerializer)
        {
            this.impressionsKafkaConfig = impressionsKafkaConfig;
        }

        protected override string EventSourcedTopicName => this.impressionsKafkaConfig.IMPRESSIONS_INTERNAL_EVENTS_TOPIC;
        protected override string FailedEventsTopicName => this.impressionsKafkaConfig.IMPRESSIONS_FAILED_EVENTS_TOPIC;
        protected override string IntegrationTopicName => this.impressionsKafkaConfig.IMPRESSIONS_INTEGRATION_EVENTS_TOPIC;
    }
}

using MidnightLizard.Impressions.Domain.FavoritesAggregate;
using MidnightLizard.Impressions.Infrastructure.Configuration;
using MidnightLizard.Impressions.Infrastructure.Serialization.Common;

namespace MidnightLizard.Impressions.Infrastructure.Queue
{
    public class FavoritesEventDispatcher : DomainEventDispatcher<FavoritesId>
    {
        private readonly FavoritesKafkaConfig impressionsKafkaConfig;

        public FavoritesEventDispatcher(
            FavoritesKafkaConfig impressionsKafkaConfig,
            KafkaConfig kafkaConfig,
            IMessageSerializer messageSerializer) : base(kafkaConfig, messageSerializer)
        {
            this.impressionsKafkaConfig = impressionsKafkaConfig;
        }

        protected override string EventSourcedTopicName => this.impressionsKafkaConfig.FAVORITES_INTERNAL_EVENTS_TOPIC;
        protected override string FailedEventsTopicName => this.impressionsKafkaConfig.FAVORITES_FAILED_EVENTS_TOPIC;
        protected override string IntegrationTopicName => this.impressionsKafkaConfig.FAVORITES_INTEGRATION_EVENTS_TOPIC;
    }
}

using MidnightLizard.SImpressionsDomain.PublicSchemeAggregate;
using MidnightLizard.Impressions.Infrastructure.Configuration;
using MidnightLizard.Impressions.Infrastructure.Serialization.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Impressions.Infrastructure.Queue
{
    public class PublicSchemeEventDispatcher : DomainEventDispatcher<PublicSchemeId>
    {
        public PublicSchemeEventDispatcher(KafkaConfig kafkaConfig, IMessageSerializer messageSerializer)
            : base(kafkaConfig, messageSerializer)
        {
        }

        protected override string GetEventTopicName() => this.kafkaConfig.IMPRESSIONS_EVENTS_TOPIC;
    }
}

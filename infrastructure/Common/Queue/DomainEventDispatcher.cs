﻿using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Commons.Domain.Interfaces;
using MidnightLizard.Commons.Domain.Messaging;
using MidnightLizard.Commons.Domain.Results;
using MidnightLizard.Impressions.Infrastructure.Configuration;
using MidnightLizard.Impressions.Infrastructure.Serialization.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidnightLizard.Impressions.Infrastructure.Queue
{
    public abstract class DomainEventDispatcher<TAggregateId> : IDomainEventDispatcher<TAggregateId>, IDisposable
        where TAggregateId : DomainEntityId
    {
        protected readonly KafkaConfig kafkaConfig;
        protected readonly IMessageSerializer messageSerializer;
        protected ISerializingProducer<string, string> producer;

        protected abstract string EventSourcedTopicName { get; }
        protected abstract string IntegrationTopicName { get; }
        protected abstract string FailedEventsTopicName { get; }

        public DomainEventDispatcher(
            KafkaConfig kafkaConfig,
            IMessageSerializer messageSerializer)
        {
            this.kafkaConfig = kafkaConfig;
            this.messageSerializer = messageSerializer;
            this.producer = new Producer<string, string>(
                kafkaConfig.KAFKA_EVENTS_PRODUCER_CONFIG,
                new StringSerializer(Encoding.UTF8),
                new StringSerializer(Encoding.UTF8));
        }

        public async Task<DomainResult> DispatchEvent(
            ITransportMessage<DomainEvent<TAggregateId>, TAggregateId> transportEvent)
        {
            try
            {
                var topicName = "";
                switch (transportEvent.Payload)
                {
                    case IntegrationEvent<TAggregateId> _:
                        topicName = this.IntegrationTopicName; break;

                    case EventSourcedDomainEvent<TAggregateId> _:
                        topicName = this.EventSourcedTopicName; break;

                    case FailedDomainEvent<TAggregateId> _:
                        topicName = this.FailedEventsTopicName; break;

                    default: break;
                }
                if (!string.IsNullOrEmpty(topicName))
                {
                    var message = this.messageSerializer.SerializeMessage(transportEvent);
                    var result = await this.producer.ProduceAsync(topicName,
                        transportEvent.Payload.AggregateId.ToString(), message);
                    if (result.Error.HasError)
                    {
                        return new DomainResult(result.Error.Reason);
                    }
                }
                return DomainResult.Ok;
            }
            catch (Exception ex)
            {
                return new DomainResult(ex);
            }
        }

        public void Dispose()
        {
            if (this.producer is IDisposable disposable)
            {
                this.producer = null;
                disposable.Dispose();
            }
        }
    }
}

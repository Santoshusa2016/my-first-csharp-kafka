using Confluent.Kafka;
using CQRS.Core.Events;
using CQRS.Core.Producers;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Post.Cmd.Infrastructure.Producers
{
    public class EventProducer : IEventProducer
    {
        private readonly ProducerConfig _config;
        public EventProducer(IOptions<ProducerConfig> config)
        {
            _config = config.Value;
        }
        public async Task ProduceAsync<TSource>(string topicName, TSource @event) where TSource : BaseEvent
        {
            //create a Kafka producer
            using var producer = new ProducerBuilder<string, string>(_config)
                .SetKeySerializer(Serializers.Utf8)
                .SetValueSerializer(Serializers.Utf8)
                .Build();

            //create new event message.
            var eventMessage = new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(),
                Value = JsonSerializer.Serialize(@event, @event.GetType())
            };

            //produce message & send to Kafka
            var deliveryResults = await producer.ProduceAsync(topicName, eventMessage);

            if (deliveryResults.Status == PersistenceStatus.NotPersisted)
            {
                throw new Exception($"could not produce {@event.GetType().Name} message to topic - {topicName} due to following reason: {deliveryResults.Message} !");
            }

        }
    }
}

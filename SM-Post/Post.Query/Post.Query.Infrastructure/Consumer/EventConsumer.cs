using Confluent.Kafka;
using CQRS.Core.Consumer;
using CQRS.Core.Events;
using Microsoft.Extensions.Options;
using Post.Query.Infrastructure.Convertor;
using Post.Query.Infrastructure.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Post.Query.Infrastructure.Consumer
{
    public class EventConsumer : IEventConsumer
    {
        private readonly ConsumerConfig _config;
        private readonly IEventHandler _eventHandler;
        public EventConsumer(IOptions<ConsumerConfig> config,
            IEventHandler eventHandler)
        {
            _config = config.Value;

            //eventhandler will consume events & save them in SQL DB
            _eventHandler = eventHandler;
        }

        public void Consume(string topic)
        {
            using var consumer = new ConsumerBuilder<string, string>(_config)
                .SetKeyDeserializer(Deserializers.Utf8)
                .SetValueDeserializer(Deserializers.Utf8)
                .Build();

            consumer.Subscribe(topic);

            while (true)
            {
                //poll kafka topic
                var consumerResult = consumer.Consume();
                if (consumerResult?.Message == null) 
                    continue;

                var options = new JsonSerializerOptions
                {
                    Converters = { new EventJSONConverter()}
                };

                //get concrete socialMedia Post event
                var @event = JsonSerializer.Deserialize<BaseEvent>(consumerResult.Message.Value, options);


                //handler method using reflection
                var handlerMethod = _eventHandler.GetType().GetMethod("On", new Type[] { @event.GetType() });

                if (handlerMethod == null)
                {
                    throw new ArgumentNullException(nameof(handlerMethod), "could not find eventHandler method");
                }

                handlerMethod.Invoke(_eventHandler, new object[] { @event });

                //notify kafka of successful handling
                consumer.Commit(consumerResult);
            }
        }
    }
}

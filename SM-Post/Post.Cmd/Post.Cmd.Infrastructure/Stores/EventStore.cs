using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Post.Cmd.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Cmd.Infrastructure.Stores
{
    public class EventStore : IEventStore
    {
        private readonly IEventStoreRepository _eventStoreRepo;
        private readonly IEventProducer _eventProducer;


        public EventStore(IEventStoreRepository eventStoreRepo, IEventProducer eventProducer)
        {
            _eventStoreRepo = eventStoreRepo;
            _eventProducer = eventProducer;
        }
        public async Task<List<BaseEvent>> GetEventsAsync(Guid aggregateId)
        {
            //get events for replaying
            var eventStream = await _eventStoreRepo.FindByAggregateID(aggregateId);
            if (eventStream == null || !eventStream.Any())
            {
                throw new AggregateNotFoundException("incorrect post id provided");
            }
            return eventStream.OrderBy(x => x.Version).Select(e => e.EventData).ToList();
        }

        public async Task SaveEventsAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion)
        {
            var eventStream = await _eventStoreRepo.FindByAggregateID(aggregateId);

            //last event should be expected version and also it must not be new => -1
            if (expectedVersion != -1 && eventStream[^1].Version != expectedVersion)
                throw new ConcurrencyException();
            
            //TBI: Add transaction scope for both saving data in DS and generating event in Kafka topic
            var version = expectedVersion;
            foreach (var @event in events)
            {
                //persist each event to the store
                @event.Version = ++version;
                var eventType = @event.GetType().Name;

                var eventModel = new EventModel()
                {
                    TimeStamp = DateTime.UtcNow,
                    AggregateID = aggregateId,
                    AggregateType = nameof(PostAggregate),
                    Version = @event.Version, //aggregate record version
                    EventType = eventType,
                    EventData = @event
                };

                await _eventStoreRepo.SaveAsync(eventModel);

                //section:08
                string topicName = Environment.GetEnvironmentVariable("KAFKA_TOPIC")?? "SocialMediaPostEvents";
                await _eventProducer.ProduceAsync(topicName, @event);
            }
        }
    }
}

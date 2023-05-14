using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
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
        public EventStore(IEventStoreRepository eventStoreRepo)
        {
            _eventStoreRepo = eventStoreRepo;
        }
        public async Task<List<BaseEvent>> GetEventsAsync(Guid aggregateId)
        {
            var eventStream = await _eventStoreRepo.FindByAggregateID(aggregateId);
            if (eventStream == null || !eventStream.Any())
            {
                throw new AggregateNotFoundException("incorrect post id provided");
            }
            return eventStream.OrderBy(x => x.Version).Select(e => e.Data).ToList();
        }

        public async Task SaveEventsAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion)
        {
            var eventStream = await _eventStoreRepo.FindByAggregateID(aggregateId);

            //last event should be expected version and also it must not be new => -1
            if (expectedVersion != -1 && eventStream[^1].Version != expectedVersion)
                throw new ConcurrencyException();
            
            var version = expectedVersion;
            foreach (var @event in events)
            {
                @event.Version = ++version;
                var eventType = @event.GetType().Name;

                var eventModel = new EventModel()
                {
                    TimeStamp = DateTime.UtcNow,
                    AggregateID = aggregateId,
                    AggregateType = nameof(PostAggregate),
                    Version = @event.Version,
                    EventType = eventType,
                    Data = @event
                };

                await _eventStoreRepo.SaveAsync(eventModel);
            }
        }
    }
}

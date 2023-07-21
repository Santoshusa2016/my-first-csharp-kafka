using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Post.Cmd.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Cmd.Infrastructure.Handlers
{
    public class EventSourcingHandler : IEventSourcingHandler<PostAggregate>
    {
        //get & save aggregate object

        private readonly IEventStore _eventStore;
        private readonly IEventProducer _eventProducer; //sect17:88
        public EventSourcingHandler(IEventStore eventStore, IEventProducer eventProducer)
        {
            _eventStore = eventStore;
            _eventProducer = eventProducer;
        }

        public async Task<PostAggregate> GetByIdAsync(Guid id)
        {
            var aggregate = new PostAggregate();

            //get all events associated with given aggregate Id
            var events = await _eventStore.GetEventsAsync(id);

            if (events == null || !events.Any()) 
                return aggregate;

            //replay all events retrived from store and rebuild the aggregate
            aggregate.ReplayEvents(events);
            aggregate.Version = events.Select(x => x.Version).Max();

            return aggregate;
        }

        public async Task SaveAsync(AggregateRoot aggregate)
        {
            await _eventStore.SaveEventsAsync(aggregate.Id,
                aggregate.GetUncommittedEvents(),
                aggregate.Version);

            //after processing events mark changes as committed
            aggregate.MarkChangesAsCommitted();
        }

        public async Task RepublishEventsAsync()
        {
            //republish all events back to Kafka: sect17:88
            var aggregateIDs = await _eventStore.GetAggegateIDsAsync();
            if (aggregateIDs == null || !aggregateIDs.Any()) return;

            foreach (var aggregateID in aggregateIDs)
            {
                var aggregate = await GetByIdAsync(aggregateID);
                if (aggregate == null || !aggregate.Active) continue;

                var events = await _eventStore.GetEventsAsync(aggregateID);
                foreach (var @event in events)
                {
                    var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
                    await _eventProducer.ProduceAsync(topic, @event);
                }
            }
        }
    }
}

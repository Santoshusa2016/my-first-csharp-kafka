using CQRS.Core.Events;
using CQRS.Core.Infrastructure;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Post.Cmd.Infrastructure.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Cmd.Infrastructure.Respositories
{
    public class EventStoreRepository : IEventStoreRepository
    {
        public IMongoCollection<EventModel> _eventStoreCollection;
        public EventStoreRepository(IOptions<MongoDbConfig> config)
        {
            var mongoClient = new MongoClient(config.Value.ConnectionString);
            var mongoDB = mongoClient.GetDatabase(config.Value.Database);
            _eventStoreCollection = mongoDB.GetCollection<EventModel>(config.Value.Collection);
        }

        public async Task<List<EventModel>> FindByAggregateID(Guid aggregateId)
        {
            return await _eventStoreCollection.Find(x => x.AggregateID == aggregateId).ToListAsync().ConfigureAwait(continueOnCapturedContext: false);   
        }

        public async Task SaveAsync(EventModel @event)
        {
            await _eventStoreCollection.InsertOneAsync(@event).ConfigureAwait(continueOnCapturedContext: false);
        }
    }
}

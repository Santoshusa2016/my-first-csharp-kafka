using CQRS.Core.Infrastructure;
using CQRS.Core.Queries;
using Post.Query.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Query.Infrastructure.Dispatchers
{
    public class QueryDispatcher : IQueryDispatcher<PostEntity>
    {
        private readonly Dictionary<Type, Func<BaseQuery, Task<List<PostEntity>>>> _handlers = new();
        public void RegisterHandler<TQuery>(Func<TQuery, Task<List<PostEntity>>> handler) where TQuery : BaseQuery
        {
            if (_handlers.ContainsKey(typeof(TQuery)))
            {
                throw new IndexOutOfRangeException("you cannot register same query handler twice");
            }
            else
            {
                //TQuery at runtime/ registration will be concrete query type
                _handlers.Add(typeof(TQuery), x => handler((TQuery)x));
            }
        }

        public async Task<List<PostEntity>> SendAsync(BaseQuery query)
        {
            if (this._handlers.TryGetValue(query.GetType(), out Func<BaseQuery, Task<List<PostEntity>>> handler))
            {
                return await handler.Invoke(query);
            }
            else
            {
                throw new ArgumentNullException(nameof(handler), "no query handler registered");
            }
        }
    }
}

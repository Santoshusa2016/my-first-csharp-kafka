using CQRS.Core.Commands;
using CQRS.Core.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRS.Core.Infrastructure
{
    //mediator
    public interface IQueryDispatcher<TEntity>
    {
        //we could also use MediatR nuget package.
        //generics-TQuery instead of BaseQuery
        void RegisterHandler<TQuery>(Func<TQuery, Task<List<TEntity>>> handler) where TQuery : BaseQuery;

        //Liskov for BaseQuery
        Task<List<TEntity>> SendAsync(BaseQuery query); //LISKOV: we could pass any of concrete query object
    }
}

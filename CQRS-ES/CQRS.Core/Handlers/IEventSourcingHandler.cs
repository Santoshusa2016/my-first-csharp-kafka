﻿using CQRS.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRS.Core.Handlers
{
    public interface IEventSourcingHandler<T>
    {
        Task SaveAsync(AggregateRoot aggregate); //save Aggregate
        Task<T> GetByIdAsync(Guid id); //return post aggregate
        Task RepublishEventsAsync(); //sect17:88
    }
}

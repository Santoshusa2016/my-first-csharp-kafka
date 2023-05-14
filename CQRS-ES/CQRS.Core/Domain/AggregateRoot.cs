using CQRS.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRS.Core.Domain
{
    public abstract class AggregateRoot
    {
        public Guid Id { get; set; }

        public int Version { get; set; } = -1;

        //events are used to make state changes to aggregate domain obj
        private readonly List<BaseEvent> _changes = new();
        
        public IEnumerable<BaseEvent> GetUncommittedEvents() => _changes;

        public void MarkChangesAsCommitted()
        {
            _changes.Clear();
        }

        private void ApplyChange(BaseEvent @event, bool isNew)
        {
            //this.GetType applies on concrete implementation since its not applicable on abstract types
            var method = this.GetType().GetMethod("Apply", new Type[] { @event.GetType() });
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method),
                    $"the apply method was not found in aggregate for {@event.GetType().Name}!");
            }
            method.Invoke(this, new object[] { @event });

            if (isNew)
            {
                //uncommitted changes not yet committed to data store
                _changes.Add(@event);
            }
        }

        protected void RaiseEvent(BaseEvent @event)
        {
            ApplyChange(@event, true);
        }

        public void ReplayEvents(IEnumerable<BaseEvent> events)
        {
            foreach (var evnt in events)
            {
                ApplyChange(evnt, false);
            }
        }
    }
}

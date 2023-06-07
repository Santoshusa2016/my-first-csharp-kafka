using CQRS.Core.Commands;
using CQRS.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Cmd.Infrastructure.Dispatchers
{
    public class CommandDispatcher : ICommandDispatcher
    {
        //concrete implementation
        private readonly Dictionary<Type, Func<BaseCommand, Task>> _handlers = new ();

        public void RegisterHandler<T>(Func<T, Task> handler) where T : BaseCommand
        {
            if (_handlers.ContainsKey(typeof(T)))
            {
                throw new InvalidOperationException("you cannot register same command handler twice");
            }
            _handlers.Add(
                typeof(T), 
                x => handler((T)x)//cast x(basecommand) to concrete type required
                ); 
        }

        public async Task SendAsync(BaseCommand command)
        {
            //on every new command raised, the handler will call app commandHandler method
            if (_handlers.TryGetValue(command.GetType(), out Func<BaseCommand, Task> handler))
            {
                await handler.Invoke(command);
            }
            else
                throw new ArgumentNullException(nameof(handler), "no command registered");
        }
    }
}

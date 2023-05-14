using CQRS.Core.Handlers;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Api.Commands
{
    public class CommandHandler : ICommandHandler
    {
        private IEventSourcingHandler<PostAggregate> _handler;

        public CommandHandler(IEventSourcingHandler<PostAggregate> handler)
        {
            _handler = handler;
        }

        public async Task HandleAsync(NewPostCommand command)
        {
            PostAggregate aggregate = new PostAggregate(
                command.Id, //commandID targets the aggregateID
                command.Author,
                command.Message
                );

            await _handler.SaveAsync(aggregate);
        }

        public async Task HandleAsync(EditMessageCommand command)
        {
            var aggregate = await _handler.GetByIdAsync(command.Id);
            aggregate.EditMessage(command.Message);

            await _handler.SaveAsync(aggregate);
        }

        public async Task HandleAsync(LikePostCommand command)
        {
            var aggregate = await _handler.GetByIdAsync(command.Id);
            aggregate.LikePost();
            await _handler.SaveAsync(aggregate);
        }

        public async Task HandleAsync(AddCommentCommand command)
        {
            var aggregate = await _handler.GetByIdAsync(command.Id);
            aggregate.AddComment(command.Comment, command.Username);
            await _handler.SaveAsync(aggregate);
        }        

        public async Task HandleAsync(EditCommentCommand command)
        {
            var aggregate = await _handler.GetByIdAsync(command.Id);
            aggregate.EditComment(command.Id,command.Comment, command.Username);
            await _handler.SaveAsync(aggregate);
        }

        public async Task HandleAsync(RemoveCommentCommand command)
        {
            var aggregate = await _handler.GetByIdAsync(command.Id);
            aggregate.RemoveComment(command.Id,command.Username);
            await _handler.SaveAsync(aggregate);
        }

        public async Task HandleAsync(DeletePostCommand command)
        {
            var aggregate = await _handler.GetByIdAsync(command.Id);
            aggregate.DeletePost(command.Username);
            await _handler.SaveAsync(aggregate);
        }
        
    }
}

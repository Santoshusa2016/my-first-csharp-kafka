using CQRS.Core.Domain;
using CQRS.Core.Messages;
using Post.Common.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Cmd.Domain.Aggregates
{
    public class PostAggregate: AggregateRoot
    {
        //an instance of PostAggregate represent a single SM post which we update via post events.
        private bool _active;

        private string _author;

        private Dictionary<Guid, Tuple<string, string>> _comments = new();

        public PostAggregate()
        {

        }
        public PostAggregate(Guid id, string author, string message)
        {
            //command handler method that also raises an event
            RaiseEvent(new PostCreatedEvent()
            {
                Id = id,
                Author = author,
                Message = message,
                DatePosted = DateTime.UtcNow
            });
        }

        public void Apply(PostCreatedEvent @event)
        {
            //method that applies raised event to aggregate
            Id = @event.Id;
            _author = @event.Author;
            _active = true;
        }

        public void EditMessage(string message)
        {
            if (!_active)
            {
                throw new InvalidOperationException("you cannot edit inactive post");
            }
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new InvalidOperationException($"the value of parameter {nameof(message)} cannot be empty");
            }
            RaiseEvent(new MessageUpdateEvent()
            {
                Id =Id,
                Message = message
            });
        }

        public void Apply (MessageUpdateEvent @event)
        {
            Id = @event.Id;
            Console.WriteLine(@event.Message);
        }

        public void LikePost()
        {
            if (!_active)
            {
                throw new InvalidOperationException("you cannot like inactive post");
            }
            RaiseEvent(new PostLikedEvent()
            {
                Id = Id
            });
        }

        public void Apply(PostLikedEvent @event)
        {
            Id = @event.Id;
        }

        public void AddComment(string comment, string username)
        {
            if (!_active)
            {
                throw new InvalidOperationException("you cannot add comment inactive post");
            }
            if (string.IsNullOrWhiteSpace(comment))
            {
                throw new InvalidOperationException($"the value of parameter {nameof(comment)} cannot be empty");
            }

            RaiseEvent(new CommentAddedEvent()
            {
                Id = Id,
                CommentId = Guid.NewGuid(),
                Comment = comment,
                Username = username,
                CommentDate = DateTime.UtcNow
            });
        }

        public void Apply(CommentAddedEvent @event)
        {
            Id = @event.Id;
            _comments.Add(@event.CommentId, new (@event.Comment, @event.Username));
        }

        public void EditComment(Guid commentId, string comment, string username)
        {
            if (!_active)
            {
                throw new InvalidOperationException("you cannot add a comment of inactive post");
            }
            if (!_comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new InvalidOperationException($"you are not allowed to comment that was made by another user");
            }

            RaiseEvent(new CommentUpdatedEvent()
            {
                Id = Id,
                CommentId = commentId,
                Comment = comment,
                Username = username,
                EditDate = DateTime.UtcNow
            });
        }

        public void Apply(CommentUpdatedEvent @event)
        {
            Id = @event.Id;
            _comments[@event.CommentId] = new(@event.Comment, @event.Username);
        }

        public void RemoveComment(Guid commentId, string username)
        {
            if (!_active)
            {
                throw new InvalidOperationException("you cannot remove a comment of an inactive post");
            }
            if (!_comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new InvalidOperationException($"you are not allowed to delete comment that was made by another user");
            }

            RaiseEvent(new CommentRemovedEvent()
            {
                Id = Id,
                CommentId = commentId                
            });
        }

        public void Apply(CommentRemovedEvent @event)
        {
            Id = @event.Id;
            _comments.Remove(@event.CommentId);
        }

        public void DeletePost(string username)
        {
            if (!_active)
            {
                throw new InvalidOperationException("you cannot delete an inactive post");
            }
            if (!_author.Equals(username, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new InvalidOperationException($"you are not allowed to delete post created by another user");
            }

            RaiseEvent(new PostRemovedEvent()
            {
                Id = Id
            });
        }

        public void Apply(PostRemovedEvent @event)
        {
            Id = @event.Id;
            _active = !_active;
        }

    }
}

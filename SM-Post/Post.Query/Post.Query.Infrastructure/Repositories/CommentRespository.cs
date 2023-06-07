using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repository;
using Post.Query.Infrastructure.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Query.Infrastructure.Repositories
{
    public class CommentRespository : ICommentRepository
    {
        private readonly DatabaseContextFactory _dbContextFactory;
        public CommentRespository(DatabaseContextFactory databaseContextFactory)
        {
            _dbContextFactory = databaseContextFactory;
        }

        public async Task CreateAsync(CommentEntity comment)
        {
            using DatabaseContext context = (DatabaseContext)_dbContextFactory.CreateDBContext();
            context.Comments.Add(comment);
            _ = await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid CommentId)
        {
            using DatabaseContext context = (DatabaseContext)_dbContextFactory.CreateDBContext();
            var comment = await GetCommentByIdAsync(CommentId);
            if (comment == null) return;
            context.Comments.Remove(comment);
            _ = await context.SaveChangesAsync();
        }

        public async Task<CommentEntity> GetCommentByIdAsync(Guid CommentId)
        {
            using DatabaseContext context = (DatabaseContext)_dbContextFactory.CreateDBContext();
            return await context.Comments.FirstOrDefaultAsync(x => x.CommentId == CommentId);
        }

        public async Task UpdateAsync(CommentEntity comment)
        {
            using DatabaseContext context = (DatabaseContext)_dbContextFactory.CreateDBContext();
            context.Comments.Update(comment);
            _ = await context.SaveChangesAsync();
        }
    }
}

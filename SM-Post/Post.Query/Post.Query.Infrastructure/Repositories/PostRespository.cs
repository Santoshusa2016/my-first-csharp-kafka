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
    internal class PostRespository : IPostRepository
    {
        private readonly DatabaseContextFactory _dbContextFactory;
        public PostRespository(DatabaseContextFactory databaseContextFactory)
        {
            _dbContextFactory = databaseContextFactory;
        }
        public async Task CreateAsync(PostEntity post)
        {
            using DatabaseContext context = (DatabaseContext)_dbContextFactory.CreateDBContext();
            context.Posts.Add(post);
            _ = await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid postId)
        {
            using DatabaseContext context = (DatabaseContext)_dbContextFactory.CreateDBContext();
            var post = await GetByIdAsyc(postId);
            if (post == null) return;
            context.Posts.Remove(post);
            _ = await context.SaveChangesAsync();
        }

        public async Task<IList<PostEntity>> GetAllLikedPostsAsyc(int noOfLikes)
        {
            using DatabaseContext context = (DatabaseContext)_dbContextFactory.CreateDBContext();
            //due to lazy loading we can return navigation props
            return await context.Posts.AsNoTracking()
                .Include(p => p.Comments).AsNoTracking()
                .Where(a => a.Likes >= noOfLikes)
                .ToListAsync();
        }

        public async Task<IList<PostEntity>> GetAllPostAsyc()
        {
            using DatabaseContext context = (DatabaseContext)_dbContextFactory.CreateDBContext();
            //due to lazy loading we can return navigation props
            return await context.Posts.AsNoTracking()
                .Include(p => p.Comments).AsNoTracking()
                .ToListAsync();
        }

        public async Task<IList<PostEntity>> GetAllPostsByAuthorsAsyc(string author)
        {
            using DatabaseContext context = (DatabaseContext)_dbContextFactory.CreateDBContext();
            //due to lazy loading we can return navigation props
            return await context.Posts.AsNoTracking()
                .Include(p => p.Comments).AsNoTracking()
                .Where(a => a.Author.Contains(author))
                .ToListAsync();
        }

        public async Task<IList<PostEntity>> GetAllPostsWithCommentsAsyc()
        {
            using DatabaseContext context = (DatabaseContext)_dbContextFactory.CreateDBContext();
            //due to lazy loading we can return navigation props
            return await context.Posts.AsNoTracking()
                .Include(p => p.Comments).AsNoTracking()
                .Where(a => a.Comments != null)
                .ToListAsync();
        }

        public async Task<PostEntity> GetByIdAsyc(Guid postId)
        {
            using DatabaseContext context = (DatabaseContext)_dbContextFactory.CreateDBContext();
            //due to lazy loading we can return navigation props
            return await context.Posts.Include(p => p.Comments).FirstOrDefaultAsync(
                x => x.PostId.Equals(postId)
                );
        }

        public async Task UpdateAsync(PostEntity post)
        {
            using DatabaseContext context = (DatabaseContext)_dbContextFactory.CreateDBContext();
            context.Posts.Update(post);

            _ = await context.SaveChangesAsync(); //discard since we dont need the return type
        }
    }
}

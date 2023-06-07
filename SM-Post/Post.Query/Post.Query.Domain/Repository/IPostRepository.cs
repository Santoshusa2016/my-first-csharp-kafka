using Post.Query.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Query.Domain.Repository
{
    public interface IPostRepository
    {
        //repository for persisting & retrieving posts
        Task CreateAsync(PostEntity post);
        Task UpdateAsync(PostEntity post);
        Task DeleteAsync(Guid postId);

        Task<PostEntity> GetByIdAsyc(Guid postId);
        Task<IList<PostEntity>> GetAllPostAsyc();
        Task<IList<PostEntity>> GetAllPostsByAuthorsAsyc(string author);
        Task<IList<PostEntity>> GetAllLikedPostsAsyc(int noOfLikes);
        Task<IList<PostEntity>> GetAllPostsWithCommentsAsyc();
    }
}

using Post.Query.Domain.Entities;
using Post.Query.Domain.Repository;

namespace Post.Query.Api.Queries
{
    public class QueryHandler : IQueryHandler
    {
        private readonly IPostRepository _postRepository;
        public QueryHandler(IPostRepository postRepository)
        {
            this._postRepository = postRepository;
        }
        public async Task<List<PostEntity>> HandleAsync(FindAllPostQuery query)
        {
            var postEntities = await _postRepository.GetAllPostAsyc();
            return postEntities.ToList();
        }

        public async Task<List<PostEntity>> HandleAsync(FindPostByIdQuery query)
        {
            var postEntity = await _postRepository.GetByIdAsyc(query.Id);
            return new List<PostEntity> { postEntity};
        }

        public async Task<List<PostEntity>> HandleAsync(FindPostByAuthorQuery query)
        {
            var postEntities = await _postRepository.GetAllPostsByAuthorsAsyc(query.Author);
            return postEntities.ToList();
        }

        public async Task<List<PostEntity>> HandleAsync(FindPostWithCommentQuery query)
        {
            var postEntities = await _postRepository.GetAllPostsWithCommentsAsyc();
            return postEntities.ToList();
        }

        public async Task<List<PostEntity>> HandleAsync(FindPostWithLikeQuery query)
        {
            var postEntities = await _postRepository.GetAllLikedPostsAsyc(query.NoOfLikes);
            return postEntities.ToList();
        }
    }
}

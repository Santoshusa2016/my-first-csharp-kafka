using CQRS.Core.Queries;

namespace Post.Query.Api.Queries
{
    public class FindPostWithLikeQuery : BaseQuery
    {
        public int NoOfLikes { get; set; }
    }
}

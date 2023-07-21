using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Common.DTOs;
using Post.Query.Api.DTOs;
using Post.Query.Api.Queries;
using Post.Query.Domain.Entities;

namespace Post.Query.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PostLookupController : ControllerBase
{
    private readonly ILogger<PostLookupController> _logger;
    private readonly IQueryDispatcher<PostEntity> _queryDispatcher;
    public PostLookupController(ILogger<PostLookupController> logger, IQueryDispatcher<PostEntity> queryDispatcher)
    {
        _logger = logger;
        _queryDispatcher = queryDispatcher;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllPostsAsync()
    {
        try
        {
            var posts = await _queryDispatcher.SendAsync(new FindAllPostQuery());
            return SuccessResponse(posts);
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }

    [HttpGet("byId/{postId}")]
    public async Task<ActionResult> GetPostByIDAsync(Guid postId)
    {
        try
        {
            var posts = await _queryDispatcher.SendAsync(new FindPostByIdQuery{ Id = postId});
            return SuccessResponse(posts);

        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }

    [HttpGet("byAuthor/{author}")]
    public async Task<ActionResult> GetPostByAuthorAsync(string author)
    {
        try
        {
            var posts = await _queryDispatcher.SendAsync(new FindPostByAuthorQuery { Author = author });
            return SuccessResponse(posts);
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }

    [HttpGet("withComments")]
    public async Task<ActionResult> GetPostsWithCommentsAsync()
    {
        try
        {
            var posts = await _queryDispatcher.SendAsync(new FindPostWithCommentQuery());
            return SuccessResponse(posts);

        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }


    [HttpGet("withLikes/{numberOfLikes}")]
    public async Task<ActionResult> GetPostsWithLikesAsync(int noOfLikes)
    {
        try
        {
            var posts = await _queryDispatcher.SendAsync(new FindPostWithLikeQuery { NoOfLikes = noOfLikes });
            return SuccessResponse(posts);

        }
        catch (Exception ex)
        {
            return ErrorResponse(ex);
        }
    }


    private ActionResult ErrorResponse(Exception ex)
    {
        const string SAFE_ERROR_MESSAGE = "error processing request";
        _logger.LogError(ex, SAFE_ERROR_MESSAGE);
        return StatusCode(500, new BaseResponse { Message = SAFE_ERROR_MESSAGE });
    }

    private ActionResult SuccessResponse(List<PostEntity> posts)
    {
        if (posts == null || posts.Any())
        {
            return NoContent();
        }
        var count = posts.Count;
        return Ok(new PostLookupResponse()
        {
            Posts = posts,
            Message = $"success returned {count} post {(count > 1 ? "s" : string.Empty)}"
        });
    }
}

using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Cmd.Api.DTOs;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class NewPostController : ControllerBase
    {
        private readonly ILogger<NewPostController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;

        public NewPostController(ILogger<NewPostController> logger, ICommandDispatcher commandDispatcher)
        {
            _logger = logger;
            _commandDispatcher = commandDispatcher;
        }

        [HttpPost]
        public async Task<ActionResult> NewPostAsync(NewPostCommand command)
        {
            var Id = Guid.NewGuid();
            command.Id = Id; //this targets aggregateId
            try
            {
                await _commandDispatcher.SendAsync(command);
                return StatusCode(StatusCodes.Status201Created, new NewPostResponse()
                {
                    Message = "new post creation completed"
                });
            }
            catch (InvalidOperationException ex)
            {
                //client has failed to pass correct details
                _logger.Log(LogLevel.Warning, ex, "client made bad request");
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch(Exception ex)
            {
                const string SAFE_ERROR = "error while processing request";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR);
                return StatusCode(StatusCodes.Status500InternalServerError, new NewPostResponse
                {
                    Id = Id,
                    Message = SAFE_ERROR
                });
            }            
        }
    }
}

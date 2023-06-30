using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AddCommentController : ControllerBase
    {
        private readonly ILogger<AddCommentController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;

        public AddCommentController(ILogger<AddCommentController> logger, ICommandDispatcher commandDispatcher)
        {
            _logger = logger;
            _commandDispatcher = commandDispatcher;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AddCommentAsync(Guid id, AddCommentCommand command)
        {
            try
            {
                command.Id = id;
                await _commandDispatcher.SendAsync(command);

                return Ok(new BaseResponse()
                {
                    Message = "Add comment added successfully"
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
            catch (AggregateNotFoundException ex)
            {
                //client has failed to pass correct details
                _logger.Log(LogLevel.Warning, ex, "could not retrieve aggregate, Client passed incorrect post id");
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR = "error while processing request";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR);
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR
                });
            }
        }
    }
}

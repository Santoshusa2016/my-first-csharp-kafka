using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Cmd.Api.DTOs;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class RestoreReadDBController : ControllerBase
    {
        private readonly ILogger<RestoreReadDBController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;

        public RestoreReadDBController(ILogger<RestoreReadDBController> logger, ICommandDispatcher commandDispatcher)
        {
            _logger = logger;
            _commandDispatcher = commandDispatcher;
        }

        [HttpPost]
        public async Task<ActionResult> RestoreReadDBAsync()
        {
            try
            {
                await _commandDispatcher.SendAsync(new RestoreReadDBCommand());
                return StatusCode(StatusCodes.Status201Created, new BaseResponse()
                {
                    Message = "read database restored request completed successfully"
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
            catch (Exception ex)
            {
                const string SAFE_ERROR = "error while processing request to restore read database";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR);
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR
                });
            }
        }
    }
}

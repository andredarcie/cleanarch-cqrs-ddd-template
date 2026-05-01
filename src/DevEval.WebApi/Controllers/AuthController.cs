using DevEval.Application.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DevEval.WebApi.Controllers
{
    /// <summary>
    /// API for authentication, providing endpoints for user login.
    /// </summary>
    [Route("api/[controller]")]
    public class AuthController : BaseController
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Authenticates a user and returns a token if the login is successful.
        /// </summary>
        /// <response code="200">Returns the authentication token.</response>
        /// <response code="401">If the credentials are invalid.</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsFailed) return MapErrors(result);

            return Ok(new { token = result.Value });
        }
    }
}

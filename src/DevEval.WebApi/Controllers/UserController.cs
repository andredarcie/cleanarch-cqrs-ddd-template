using DevEval.Application.Users.Commands;
using DevEval.Application.Users.Dtos;
using DevEval.Application.Users.Queries;
using DevEval.Common.Helpers.Pagination;
using DevEval.WebApi.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevEval.WebApi.Controllers
{
    /// <summary>
    /// API for managing Users, including retrieval, creation, updating, and deletion.
    /// </summary>
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieve a paginated list of Users.
        /// </summary>
        /// <response code="200">Returns the paginated list of Users.</response>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResult<UserDto>), 200)]
        public async Task<IActionResult> GetUsers([FromQuery] UserQueryRequest request)
        {
            var result = await _mediator.Send(new GetUsersQuery(
                new PaginationParameters
                {
                    Page = request.Page <= 0 ? 1 : request.Page,
                    PageSize = request.Size <= 0 ? 10 : request.Size,
                    OrderBy = request.Order
                },
                request.Username,
                request.Email,
                request.Status,
                request.Role));

            return HandleResult(result);
        }

        /// <summary>
        /// Retrieve a specific User by ID.
        /// </summary>
        /// <response code="200">Returns the User.</response>
        /// <response code="404">If the User is not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserById(int id)
        {
            var result = await _mediator.Send(new GetUserByIdQuery(id));
            return HandleResult(result);
        }

        /// <summary>
        /// Add a new User.
        /// </summary>
        /// <response code="201">Returns the created User.</response>
        [HttpPost]
        [ProducesResponseType(typeof(UserDto), 201)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsFailed) return MapErrors(result);

            return CreatedAtAction(nameof(GetUserById), new { id = result.Value.Id }, result.Value);
        }

        /// <summary>
        /// Update an existing User by ID.
        /// </summary>
        /// <response code="200">Returns the updated User.</response>
        /// <response code="400">If the ID in the path does not match the ID in the body.</response>
        /// <response code="404">If the User is not found.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserCommand command)
        {
            if (id != command.Id)
                return BadRequest(new { type = "ValidationError", error = "Validation error", detail = "ID in path and body do not match." });

            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>
        /// Delete a User by ID.
        /// </summary>
        /// <response code="200">If the User is deleted successfully.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _mediator.Send(new DeleteUserCommand(id));
            if (result.IsFailed) return MapErrors(result);
            return Ok(new { message = "User deleted successfully" });
        }
    }
}

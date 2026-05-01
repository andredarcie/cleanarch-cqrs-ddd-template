using Microsoft.AspNetCore.Mvc;
using MediatR;
using DevEval.Application.Carts.Commands;
using DevEval.Common.Helpers.Pagination;
using DevEval.Application.Carts.Queries;
using DevEval.Application.Carts.Dtos;
using DevEval.Application.Sales.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace DevEval.WebApi.Controllers
{
    /// <summary>
    /// API for managing Carts, including retrieval, creation, updating, deletion, and checkout.
    /// </summary>
    [Route("api/[controller]")]
    [Authorize]
    public class CartsController : BaseController
    {
        private readonly IMediator _mediator;

        public CartsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves a list of all carts with optional pagination and sorting.
        /// </summary>
        /// <response code="200">Returns the paginated list of carts.</response>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResult<CartDto>), 200)]
        public async Task<IActionResult> GetCarts(
            [FromQuery] int _page = 1,
            [FromQuery] int _size = 10,
            [FromQuery] string _order = "")
        {
            var result = await _mediator.Send(new GetCartsQuery(new PaginationParameters
            {
                Page = _page,
                PageSize = _size,
                OrderBy = _order
            }));

            return HandleResult(result);
        }

        /// <summary>
        /// Retrieves a specific cart by ID.
        /// </summary>
        /// <response code="200">Returns the cart.</response>
        /// <response code="404">If the cart is not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CartDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCartById(int id)
        {
            var result = await _mediator.Send(new GetCartByIdQuery(id));
            return HandleResult(result);
        }

        /// <summary>
        /// Adds a new cart for the authenticated user.
        /// </summary>
        /// <response code="201">Returns the created cart.</response>
        /// <response code="401">If the user is not authenticated or the UserId claim is missing.</response>
        /// <response code="400">If the UserId claim is in an invalid format.</response>
        [HttpPost]
        [ProducesResponseType(typeof(CartDto), 201)]
        [ProducesResponseType(401)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateCart([FromBody] CreateCartCommand command)
        {
            var userIdClaim = User.FindFirst("UserId");

            if (userIdClaim == null)
                return Unauthorized(new { type = "AuthenticationError", error = "Authentication error", detail = "UserId claim is missing in the token." });

            if (!int.TryParse(userIdClaim.Value, out var userId))
                return BadRequest(new { type = "ValidationError", error = "Validation error", detail = "Invalid UserId format in the token." });

            command.UserId = userId;

            var result = await _mediator.Send(command);

            if (result.IsFailed) return MapErrors(result);

            return CreatedAtAction(nameof(GetCartById), new { id = result.Value.Id }, result.Value);
        }

        /// <summary>
        /// Updates an existing cart by ID.
        /// </summary>
        /// <response code="200">Returns the updated cart.</response>
        /// <response code="400">If the ID in the path does not match the ID in the command.</response>
        /// <response code="404">If the cart is not found.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(CartDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateCart(int id, [FromBody] UpdateCartCommand command)
        {
            if (id != command.Id)
                return BadRequest(new { type = "ValidationError", error = "Validation error", detail = "ID in path and body do not match." });

            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>
        /// Deletes a cart by ID.
        /// </summary>
        /// <response code="200">If the cart is deleted successfully.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> DeleteCart(int id)
        {
            var result = await _mediator.Send(new DeleteCartCommand(id));
            if (result.IsFailed) return MapErrors(result);
            return Ok(new { Message = "Cart deleted successfully" });
        }

        /// <summary>
        /// Converts a cart into a sale (checkout process).
        /// </summary>
        /// <response code="201">Returns the created sale.</response>
        /// <response code="404">If the cart is not found or cannot be converted to a sale.</response>
        [HttpPost("{id}/checkout")]
        [ProducesResponseType(typeof(SaleDto), 201)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CheckoutCart(int id)
        {
            var result = await _mediator.Send(new ConvertCartToSaleCommand(id));

            if (result.IsFailed) return MapErrors(result);

            return CreatedAtAction("GetSaleById", "Sales", new { id = result.Value.Id }, result.Value);
        }
    }
}

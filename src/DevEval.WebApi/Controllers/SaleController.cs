using Microsoft.AspNetCore.Mvc;
using MediatR;
using DevEval.Common.Helpers.Pagination;
using DevEval.Application.Sales.Dtos;
using DevEval.Application.Sales.Commands;
using DevEval.Application.Sales.Queries;
using Microsoft.AspNetCore.Authorization;

namespace DevEval.WebApi.Controllers
{
    /// <summary>
    /// API for managing Sales, including retrieval, creation, updating, and deletion of sales.
    /// </summary>
    [Route("api/[controller]")]
    [Authorize]
    public class SalesController : BaseController
    {
        private readonly IMediator _mediator;

        public SalesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieve a paginated list of Sales.
        /// </summary>
        /// <response code="200">Returns the paginated list of Sales.</response>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResult<SaleDto>), 200)]
        public async Task<IActionResult> GetSales(
            [FromQuery] int _page = 1,
            [FromQuery] int _size = 10,
            [FromQuery] string _order = "")
        {
            var result = await _mediator.Send(new GetSalesQuery(new PaginationParameters
            {
                Page = _page,
                PageSize = _size,
                OrderBy = _order
            }));

            return HandleResult(result);
        }

        /// <summary>
        /// Retrieve a specific Sale by ID.
        /// </summary>
        /// <response code="200">Returns the Sale.</response>
        /// <response code="404">If the Sale is not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SaleDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetSaleById(Guid id)
        {
            var result = await _mediator.Send(new GetSaleByIdQuery(id));
            return HandleResult(result);
        }

        /// <summary>
        /// Add a new Sale.
        /// </summary>
        /// <response code="201">Returns the created Sale.</response>
        [HttpPost]
        [ProducesResponseType(typeof(SaleDto), 201)]
        public async Task<IActionResult> CreateSale([FromBody] CreateSaleCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsFailed) return MapErrors(result);

            return CreatedAtAction(nameof(GetSaleById), new { id = result.Value.Id }, result.Value);
        }

        /// <summary>
        /// Update an existing Sale by ID.
        /// </summary>
        /// <response code="200">Returns the updated Sale.</response>
        /// <response code="400">If the ID in the path does not match the ID in the body.</response>
        /// <response code="404">If the Sale is not found.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(SaleDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateSale(Guid id, [FromBody] UpdateSaleCommand command)
        {
            if (id != command.Id)
                return BadRequest(new { type = "ValidationError", error = "Validation error", detail = "ID in path and body do not match." });

            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>
        /// Delete a Sale by ID.
        /// </summary>
        /// <response code="200">If the Sale is deleted successfully.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> DeleteSale(Guid id)
        {
            var result = await _mediator.Send(new DeleteSaleCommand(id));
            if (result.IsFailed) return MapErrors(result);
            return Ok(new { Message = "Sale deleted successfully" });
        }

        /// <summary>
        /// Cancel a specific item in a Sale.
        /// </summary>
        /// <response code="200">If the item is cancelled successfully.</response>
        /// <response code="404">If the sale or item is not found.</response>
        [HttpPut("{saleId}/items/{itemId}/cancel")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CancelSaleItem(Guid saleId, Guid itemId, [FromBody] string reason)
        {
            var result = await _mediator.Send(new CancelSaleItemCommand(saleId, itemId, reason));
            if (result.IsFailed) return MapErrors(result);
            return Ok(new { Message = "Item cancelled successfully." });
        }
    }
}

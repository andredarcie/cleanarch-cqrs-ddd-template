using DevEval.Application.Products.Commands;
using DevEval.Application.Products.Dtos;
using DevEval.Application.Products.Queries;
using DevEval.Common.Helpers.Pagination;
using DevEval.WebApi.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevEval.API.Controllers
{
    /// <summary>
    /// API for managing Products, including retrieval, creation, updating, and deletion of products.
    /// </summary>
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : BaseController
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves all products with pagination.
        /// </summary>
        /// <response code="200">Returns the paginated list of products.</response>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResult<ProductDto>), 200)]
        public async Task<IActionResult> GetProducts(
            [FromQuery] int _page = 1,
            [FromQuery] int _size = 10,
            [FromQuery] string _order = "")
        {
            var result = await _mediator.Send(new GetProductsQuery(new PaginationParameters
            {
                Page = _page,
                PageSize = _size,
                OrderBy = _order
            }));

            return HandleResult(result);
        }

        /// <summary>
        /// Retrieves a product by ID.
        /// </summary>
        /// <response code="200">Returns the product.</response>
        /// <response code="404">If the product is not found.</response>
        [HttpGet("categories")]
        [ProducesResponseType(typeof(IEnumerable<string>), 200)]
        public async Task<IActionResult> GetCategories()
        {
            var result = await _mediator.Send(new GetCategoriesQuery());
            return HandleResult(result);
        }

        /// <summary>
        /// Retrieves a product by ID.
        /// </summary>
        /// <response code="200">Returns the product.</response>
        /// <response code="404">If the product is not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetProductById(int id)
        {
            var result = await _mediator.Send(new GetProductByIdQuery(id));
            return HandleResult(result);
        }

        /// <summary>
        /// Retrieves products by category with pagination.
        /// </summary>
        /// <response code="200">Returns the paginated list of products in the category.</response>
        [HttpGet("category/{category}")]
        [ProducesResponseType(typeof(PaginatedResult<ProductDto>), 200)]
        public async Task<IActionResult> GetProductsByCategory(string category, [FromQuery] PaginationParameters parameters)
        {
            var result = await _mediator.Send(new GetProductsByCategoryQuery(category, parameters));
            return HandleResult(result);
        }

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <response code="201">Returns the created product.</response>
        /// <response code="400">If the product data is invalid.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ProductDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsFailed) return MapErrors(result);

            return CreatedAtAction(nameof(GetProductById), new { id = result.Value.Id }, result.Value);
        }

        /// <summary>
        /// Updates an existing product by ID.
        /// </summary>
        /// <response code="200">Returns the updated product.</response>
        /// <response code="400">If the product data is invalid.</response>
        /// <response code="404">If the product is not found.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ProductDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>
        /// Deletes a product by ID.
        /// </summary>
        /// <response code="204">If the product is deleted successfully.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _mediator.Send(new DeleteProductCommand(id));
            if (result.IsFailed) return MapErrors(result);
            return NoContent();
        }
    }
}

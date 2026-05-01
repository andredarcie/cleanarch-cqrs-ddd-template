using DevEval.Application.Common.Mappings;
using DevEval.Application.Carts.Dtos;
using DevEval.Application.Carts.Queries;
using DevEval.Common.Helpers.Pagination;
using DevEval.Domain.Repositories;
using FluentResults;
using MediatR;

namespace DevEval.Application.Carts.Handlers
{
    public class GetCartsHandler : IRequestHandler<GetCartsQuery, Result<PaginatedResult<CartDto>>>
    {
        private readonly ICartRepository _repository;

        public GetCartsHandler(ICartRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<PaginatedResult<CartDto>>> Handle(GetCartsQuery request, CancellationToken cancellationToken)
        {
            var result = await _repository.GetAllWithProductsAsync(request.Parameters);

            return Result.Ok(new PaginatedResult<CartDto>
            {
                Items = result.Items.Select(cart => cart.ToDto()).ToList(),
                TotalItems = result.TotalItems,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages
            });
        }
    }
}

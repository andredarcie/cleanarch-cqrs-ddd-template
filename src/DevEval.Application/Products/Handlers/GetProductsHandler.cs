using AutoMapper;
using DevEval.Application.Products.Dtos;
using DevEval.Application.Products.Queries;
using DevEval.Common.Helpers.Pagination;
using DevEval.Domain.Repositories;
using FluentResults;
using MediatR;

namespace DevEval.Application.Products.Handlers
{
    public class GetProductsHandler : IRequestHandler<GetProductsQuery, Result<PaginatedResult<ProductDto>>>
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;

        public GetProductsHandler(IProductRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<PaginatedResult<ProductDto>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var result = await _repository.GetAllAsync(request.Parameters);

            return Result.Ok(new PaginatedResult<ProductDto>
            {
                Items = _mapper.Map<List<ProductDto>>(result.Items),
                TotalItems = result.TotalItems,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages
            });
        }
    }
}

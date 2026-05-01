using DevEval.Application.Products.Queries;
using DevEval.Domain.Repositories;
using FluentResults;
using MediatR;

namespace DevEval.Application.Products.Handlers
{
    public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, Result<IEnumerable<string>>>
    {
        private readonly IProductRepository _repository;

        public GetCategoriesHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<IEnumerable<string>>> Handle(GetCategoriesQuery query, CancellationToken cancellationToken)
        {
            var categories = await _repository.GetCategoriesAsync();
            return Result.Ok(categories);
        }
    }
}

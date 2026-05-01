using DevEval.Application.Common.Mappings;
using DevEval.Application.Common.Errors;
using DevEval.Application.Products.Commands;
using DevEval.Application.Products.Dtos;
using DevEval.Domain.Repositories;
using FluentResults;
using MediatR;

namespace DevEval.Application.Products.Handlers
{
    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, Result<ProductDto>>
    {
        private readonly IProductRepository _repository;

        public UpdateProductHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<ProductDto>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var existingProduct = await _repository.GetByIdAsync(request.Id);

            if (existingProduct == null)
                return Result.Fail(new NotFoundError($"Product with ID {request.Id} not found."));

            request.ApplyTo(existingProduct);

            var updatedProduct = await _repository.UpdateAsync(existingProduct);

            return Result.Ok(updatedProduct.ToDto());
        }
    }
}

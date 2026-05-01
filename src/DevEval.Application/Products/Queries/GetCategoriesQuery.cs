using FluentResults;
using MediatR;

namespace DevEval.Application.Products.Queries
{
    public class GetCategoriesQuery : IRequest<Result<IEnumerable<string>>>
    {
    }
}

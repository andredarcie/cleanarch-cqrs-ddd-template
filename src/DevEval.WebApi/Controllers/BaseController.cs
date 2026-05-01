using DevEval.Application.Common.Errors;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace DevEval.WebApi.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult HandleResult<T>(Result<T> result)
        {
            if (result.IsSuccess) return Ok(result.Value);
            return MapErrors(result);
        }

        protected IActionResult HandleResult(Result result)
        {
            if (result.IsSuccess) return Ok();
            return MapErrors(result);
        }

        protected IActionResult MapErrors(IResultBase result)
        {
            var error = result.Errors.FirstOrDefault();
            return error switch
            {
                NotFoundError => NotFound(new { type = "ResourceNotFound", error = "Resource not found", detail = error.Message }),
                UnauthorizedError => Unauthorized(new { type = "AuthenticationError", error = "Authentication error", detail = error.Message }),
                ValidationError => BadRequest(new { type = "ValidationError", error = "Validation error", detail = error.Message }),
                _ => StatusCode(500, new { type = "InternalServerError", error = "Internal server error", detail = error?.Message })
            };
        }
    }
}

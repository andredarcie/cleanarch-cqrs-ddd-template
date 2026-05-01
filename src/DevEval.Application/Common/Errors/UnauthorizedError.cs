using FluentResults;

namespace DevEval.Application.Common.Errors
{
    public class UnauthorizedError : Error
    {
        public UnauthorizedError(string message) : base(message) { }
    }
}

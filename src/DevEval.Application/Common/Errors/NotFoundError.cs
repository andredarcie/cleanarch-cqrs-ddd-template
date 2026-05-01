using FluentResults;

namespace DevEval.Application.Common.Errors
{
    public class NotFoundError : Error
    {
        public NotFoundError(string message) : base(message) { }
    }
}

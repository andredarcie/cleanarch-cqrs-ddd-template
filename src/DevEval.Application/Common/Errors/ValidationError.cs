using FluentResults;

namespace DevEval.Application.Common.Errors
{
    public class ValidationError : Error
    {
        public ValidationError(string message) : base(message) { }
    }
}

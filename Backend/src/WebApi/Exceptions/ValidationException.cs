using FluentValidation.Results;

namespace WebApi.Exceptions;

public class ValidationException : UserFacingException
{
    private IEnumerable<ValidationFailure> Errors { get; }
    public ValidationException(IEnumerable<ValidationFailure> errors)
        :base("")
    {
        Errors = errors;
    }
}